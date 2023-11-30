using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
    public class Health : NetworkBehaviour
    {
    
        [field: SerializeField] public int MaxHealth { get; private set; }
        public NetworkVariable<int> currentHealth;

        private bool _isDead;
        public event Action<Health> OnDead;
    
    
        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
        
            currentHealth.Value = MaxHealth;
        }


        public void TakeDamage(int damage)
        {
            HandleHealth(-damage);
        }

        public void Heal(int health)
        {
            HandleHealth(health);
        }

        private void HandleHealth(int health)
        {

            if (_isDead) return;

            int newHealth = Math.Clamp(currentHealth.Value + health, 0, MaxHealth);
            currentHealth.Value = newHealth;

            if (currentHealth.Value == 0)
            {
                OnDead?.Invoke(this);
                _isDead = true;
            }


        }
    
    
    
    
    
    
    
    }
}
