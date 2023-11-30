using System.Collections;
using System.Collections.Generic;
using Core.Combat;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{

    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private int keptCoinPercentage = 50;
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return;}

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawn(player);
        }
        
        TankPlayer.OnPlayerSpawned += HandlePlayerSpawn;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawn;
        
    }

    
    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return;}

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawn;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawn;
    }
    
    private void HandlePlayerSpawn(TankPlayer player)
    {
        player.Health.OnDead += (health) => HandleDie(player);
    }

    
    private void HandlePlayerDespawn(TankPlayer player)
    {
        player.Health.OnDead -= (health) => HandleDie(player);
    }


    private void HandleDie(TankPlayer tankPlayer)
    {
        int remainingCoin = tankPlayer.CoinWallet.TotalCoins.Value * (keptCoinPercentage / 100);
        Debug.Log(remainingCoin + "Remaining coin");
        Destroy(tankPlayer.gameObject);

        StartCoroutine(PlayerRespawn(tankPlayer.OwnerClientId,remainingCoin));

    }

    private IEnumerator PlayerRespawn(ulong clientId, int remainingCoin)
    {
        yield return new WaitForSeconds(2);
        
        TankPlayer playerInstance = Instantiate(
            playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(clientId);
        playerInstance.CoinWallet.TotalCoins.Value += remainingCoin;

    }

 
    
    
}
