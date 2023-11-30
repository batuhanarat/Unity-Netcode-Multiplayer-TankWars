using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Core.Coins;
using Core.Combat;
using Unity.Collections;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private SpriteRenderer minimapIcon;
    [field : SerializeField] public Health Health { get; private set; }
    [field : SerializeField] public CoinWallet CoinWallet { get; private set; }

    
    [Header("Settings")]
    [SerializeField] private  int priorityNumber = 15;

    [SerializeField] private Color playerColor;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
           UserData data = HostSingleton.Instance.HostGameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
           playerName.Value = data.userName;

           OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            _virtualCamera.Priority = priorityNumber;
            minimapIcon.color = playerColor;

        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
