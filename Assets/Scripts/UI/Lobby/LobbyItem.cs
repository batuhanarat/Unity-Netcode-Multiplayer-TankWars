using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text LobbyNameText;
    [SerializeField] private TMP_Text LobbySizeText;

    private LobbiesList _lobbiesList;
    private Lobby _lobby;    

    public void Initialize(LobbiesList lobbiesList, Lobby lobby)
    {
        this._lobbiesList = lobbiesList;
        this._lobby = lobby;

        LobbyNameText.text = lobby.Name;
        LobbySizeText.text = lobby.Players.Count + "/" + lobby.MaxPlayers; 
        
    }

    public void Join()
    {
        _lobbiesList.JoinAsync(_lobby);
    }
    

    
}
