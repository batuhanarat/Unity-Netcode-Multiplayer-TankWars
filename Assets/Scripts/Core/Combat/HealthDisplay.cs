using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Combat
{
    public class HealthDisplay : NetworkBehaviour
    {
   
        
        [Header("References")]
        [SerializeField] private Health health;
        [SerializeField] private Image _healthBar;
        

    //OnValueChange is the built in event that needs argument with old value and new value
    //Since it is just about UI, we dont need to exhaust the server.
        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;
            health.currentHealth.OnValueChanged += HandleHealthChange;
            HandleHealthChange(0,100);
        }

      

        public override void OnNetworkDespawn()
        {
            if (!IsClient) return;
            health.currentHealth.OnValueChanged -= HandleHealthChange;
        }
        
        private void HandleHealthChange(int oldHealth, int newHealth)
        {

            _healthBar.fillAmount = (float)newHealth / health.MaxHealth;
        }
    }
}
