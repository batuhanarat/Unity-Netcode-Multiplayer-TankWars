using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.Leaderboard;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Leaderboard : NetworkBehaviour
{
    [SerializeField] private Transform leaderboardEntityParentTransform;
    [SerializeField] private LeaderboardEntityDisplay leaderboardEntityPrefab;
    [SerializeField] private int leaderboardSizeAsVisible;


    private NetworkList<LeaderboardEntityStruct> leaderboardEntities;
    private List<LeaderboardEntityDisplay> entityDisplays = new List<LeaderboardEntityDisplay>();
    

    private void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityStruct>();
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;
            foreach(LeaderboardEntityStruct entity in leaderboardEntities)
            {
                HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntityStruct>
                {
                    Type = NetworkListEvent<LeaderboardEntityStruct>.EventType.Add,
                    Value = entity
                });
            }


        }


        if (IsServer)
        {
            
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }
        
            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
            
            
        }

        
    }

 

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;

        }
        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }

    }
    private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntityStruct> changeEvent)
    {
        switch (changeEvent.Type)
        {
                case NetworkListEvent<LeaderboardEntityStruct>.EventType.Add:
                    if (!entityDisplays.Any(x => x.ClientId == changeEvent.Value.PlayerID))
                    {
                        LeaderboardEntityDisplay entityToAdd =
                            Instantiate(leaderboardEntityPrefab, leaderboardEntityParentTransform);
                        
                        entityToAdd.Initialise(
                            changeEvent.Value.PlayerID,
                            changeEvent.Value.PlayerName,
                            changeEvent.Value.Coins);
                        
                        entityDisplays.Add(entityToAdd);
                        
                    }
                    break;
                
                case NetworkListEvent<LeaderboardEntityStruct>.EventType.Remove:
                    LeaderboardEntityDisplay entityToRemove =
                        entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.PlayerID);

                    if (entityToRemove != null)
                    {
                        /*it is for being cautious.
                        Sometimes it gives error at destroy. So by setting its parent to null, we are first isolating it, then destroy it.
                        Which can prevent error */
                        
                        entityToRemove.transform.SetParent(null);
                        
                        Destroy(entityToRemove.gameObject);
                        entityDisplays.Remove(entityToRemove);
                        
                    }
                    break;
                
                case NetworkListEvent<LeaderboardEntityStruct>.EventType.Value:
                    LeaderboardEntityDisplay entityToUpdate =
                        entityDisplays.FirstOrDefault(x => x.ClientId == changeEvent.Value.PlayerID);
                    if (entityToUpdate != null)
                    {
                        entityToUpdate.UpdateCoins(changeEvent.Value.Coins);
                    }

                    break;
        }

     entityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));

        for ( int i = 0 ; i< entityDisplays.Count ; i++ )
        {
            entityDisplays[i].transform.SetSiblingIndex(i);
            entityDisplays[i].UpdateText();
            
            if (i < leaderboardSizeAsVisible - 1)
            {
                entityDisplays[i].gameObject.SetActive(true);
            }
        }
        
        LeaderboardEntityDisplay myDisplay =
            entityDisplays.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        if (myDisplay != null)
        {
            if (myDisplay.transform.GetSiblingIndex() >= leaderboardSizeAsVisible)
            {
                leaderboardEntityParentTransform.GetChild(leaderboardSizeAsVisible - 1).gameObject.SetActive(false);
                myDisplay.gameObject.SetActive(true);
            }
        }

        
        
        
        
        
        
        

    }


    private void HandlePlayerSpawned(TankPlayer player)
    {
        leaderboardEntities.Add(
            new LeaderboardEntityStruct()
            {
                PlayerID = player.OwnerClientId,
                PlayerName = player.playerName.Value,
                Coins = player.CoinWallet.TotalCoins.Value
            });

        player.CoinWallet.TotalCoins.OnValueChanged += (oldCoin, newCoin) => UpdateCoin(player.OwnerClientId, newCoin);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        
        foreach (LeaderboardEntityStruct entity in leaderboardEntities)
        {
            if (entity.PlayerID != player.OwnerClientId) { continue; }

            leaderboardEntities.Remove(entity);
            break;
        }

        player.CoinWallet.TotalCoins.OnValueChanged -= (oldCoin, newCoin) => UpdateCoin(player.OwnerClientId, newCoin);


    }

    private void UpdateCoin(ulong clientId, int newCoins)
    {
        for (int i = 0; i < leaderboardEntities.Count; i++)
        {
            if (leaderboardEntities[i].PlayerID != clientId) { continue; }

            leaderboardEntities[i] = new LeaderboardEntityStruct()
            {
                PlayerID = clientId,
                PlayerName =  leaderboardEntities[i].PlayerName,
                Coins = newCoins
            };
            
            break;
        }
    }

  
}
