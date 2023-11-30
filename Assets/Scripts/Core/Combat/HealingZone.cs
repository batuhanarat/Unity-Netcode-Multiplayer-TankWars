using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    
    [Header("References")]
    [SerializeField] private Image healPowerBar;

    [Header("Settings")]
    [SerializeField] private int maxHealPower = 30;
    [SerializeField] private float healCooldown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private int coinsPerTick = 10;
    [SerializeField] private int healthPerTick = 10;
    
    private float remainingCooldown;
    private float tickTimer;
    private List<TankPlayer> playersInZone = new List<TankPlayer>();


    private NetworkVariable<int> HealPower = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealthbarChanged;
            HandleHealthbarChanged(0, HealPower.Value);
        }

        if (IsServer)
        {
            HealPower.Value = maxHealPower;
        }
        
    }

  

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealthbarChanged;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) { return; }

        if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer playerInZone))
        {
            playersInZone.Add(playerInZone);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) { return; }
        if (other.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer playerInZone))
        {
            if (playersInZone.Contains(playerInZone))
            {
                playersInZone.Remove(playerInZone);

            }
        }
    }
    
    
    private void HandleHealthbarChanged(int oldHealValue, int newHealValue)
    {
        healPowerBar.fillAmount = (float)newHealValue/ maxHealPower;
    }

    private void Update()
    {
        if (!IsServer) { return; }

        //Cooldown
        if (remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;
            
            if (remainingCooldown <= 0f)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }
        
        //Heal
        tickTimer += Time.deltaTime;
        if (tickTimer >= 1/healTickRate)
        {
            
            foreach (TankPlayer player in playersInZone) 
            {
                //If it already has max health
                //If it has no coin enough
                //If there is enough power to heal in zone
                if (HealPower.Value == 0) { break; }
                if (player.Health.currentHealth.Value == player.Health.MaxHealth) { continue; }
                if (player.CoinWallet.TotalCoins.Value < coinsPerTick) { continue; }
                
                player.CoinWallet.SpendCoins(coinsPerTick);
                player.Health.Heal(healthPerTick);

                HealPower.Value -= 1;

                if (HealPower.Value == 0)
                {
                    remainingCooldown = healCooldown;
                }
            }

            tickTimer = tickTimer % (1/healTickRate);
            

        }
        
        
        
        
        
        
    }
}
