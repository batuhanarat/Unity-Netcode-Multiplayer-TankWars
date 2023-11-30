using Unity.Netcode;
using UnityEngine;

namespace Core.Combat
{
    public class DealDamageOnContact : MonoBehaviour
    {

        [SerializeField] private int damageValue;
        private ulong ownerClientID;

    
        public void SetOwner(ulong id)
        {
            this.ownerClientID = id;
        }
    

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.attachedRigidbody == null) { return; }
        
         
            if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
            {
                if (netObj.OwnerClientId == ownerClientID) {return;}
                
            }
            
            
            if (col.attachedRigidbody.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damageValue);
            }
        

        }
    }
}
