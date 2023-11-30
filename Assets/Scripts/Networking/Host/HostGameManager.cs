using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class HostGameManager: IDisposable
{

  private const int MaxConnection = 20;
  private const string GameSceneName = "Game";

  public  NetworkServer NetworkServer { get; private set; }
  private Allocation _allocation;
  private string joinCode;
  private String lobbyId;
  
  
  public async Task StartHostAsync()
  {

    try
    {
      _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnection);
      
    }
    catch (Exception exp)
    {
      Debug.Log("Error on allocation " + exp);
      return;
    }

    try
    {
       joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
      Debug.Log("Join code: " +joinCode );

    }
    catch (Exception exp)
    {
      Debug.Log("Error on join code " +exp );
    }

    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
    
    transport.SetRelayServerData(relayServerData);


    try
    {
      CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
      lobbyOptions.IsPrivate = false;
      lobbyOptions.Data = new Dictionary<string, DataObject>()
        {
          {
            "JoinCode", new DataObject(
              visibility: DataObject.VisibilityOptions.Member,
              value: joinCode )

          }
          
        };
      
      String playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unknown Lobby");
      Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(playerName+ "'s Lobby", MaxConnection, lobbyOptions);
      lobbyId = lobby.Id;

      //HeartbeatPing(15);
       HostSingleton.Instance.StartCoroutine(HeartbeatPing(15));

    }
    catch (LobbyServiceException lobbyException)
    {
      Debug.LogError(lobbyException);
      return;
    }

    NetworkServer = new NetworkServer(NetworkManager.Singleton);
    
    UserData userData = new UserData
    {
      userName =  PlayerPrefs.GetString(NameSelector.PlayerNameKey,"Missing Name"),
      userAuthId = AuthenticationService.Instance.PlayerId

    };

    string payload = JsonUtility.ToJson(userData);
    byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
    NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
    
    NetworkManager.Singleton.StartHost();
    NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);



  }
/*
  private async Task HeartbeatPing(float waitTimeSeconds)
  {

    WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
    while (true)
    {
      await  Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
      await Task.Delay((int) waitTimeSeconds *1000);
    }
    
  }
  */
    
  private IEnumerator HeartbeatPing(float waitTimeSeconds)
  {

    WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
    while (true)
    {
        Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
        yield return delay;
    }
    
  }


  public async void Dispose()
  {
    HostSingleton.Instance.StopCoroutine(nameof(HeartbeatPing));

    if (!string.IsNullOrEmpty(lobbyId))
    {

      try
      {
        await Lobbies.Instance.DeleteLobbyAsync(lobbyId);

      }
      catch (LobbyServiceException e)
      {
        Debug.LogError(e);        
      }

      lobbyId = string.Empty;

    }
    NetworkServer?.Dispose();
     


  }
}
