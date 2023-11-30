using Core.Coins;
using Core.Combat;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Player
{
    public class ProjectileLauncher : NetworkBehaviour
    {

        [Header("References")]
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private GameObject serverProjectilePrefab;
        [SerializeField] private GameObject clientProjectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject muzzleFire;
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private CoinWallet wallet;

        [Header("Settings")]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float fireRate;
        [SerializeField] private float muzzleFireDuration;
        [SerializeField] private int costToFire;
        
        private bool _canFire;
        private float _muzzleFireTimer;
        private float timer;


        
        
        public override void OnNetworkSpawn()
        {
            _inputReader.PrimaryFiredEvent += HandlePrimaryFire;
            
            
        }

       
        public override void OnNetworkDespawn()
        {
            _inputReader.PrimaryFiredEvent -= HandlePrimaryFire;

        }
        
        private void HandlePrimaryFire(bool canFire)
        {
            _canFire = canFire;
        }


        // Update is called once per frame
        void Update()
        {
            if (_muzzleFireTimer > 0)
            {
                _muzzleFireTimer -= Time.deltaTime;
                if (_muzzleFireTimer <= 0)
                {
                    muzzleFire.SetActive(false);
                }
            } 
            
            if(!IsOwner) {return;}
            
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            
            if(!_canFire) {return;}
            if (timer > 0) { return; }
            if (wallet.TotalCoins.Value < costToFire) { return; }


            SpawnProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
            SpawnProjectileServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

            timer = 1 / fireRate;
        }

        //This method is for quick reaction to mimic smoothness in the game.
        //It draw a projectile, but the logic is empty because we need to take the validation from the server.
        //While we are waiting for the validation, to give smooth effect that illusion that everything happens so fast,
        //we fire a dummy projectile for just visual effect.
        private void SpawnProjectile(Vector3 position, Vector3 direction)
        {
            
            _muzzleFireTimer = muzzleFireDuration;
            muzzleFire.SetActive(true);

            var prefabInstance = Instantiate(clientProjectilePrefab,
                position,
                Quaternion.identity);

            
            prefabInstance.transform.up = direction;
            Physics2D.IgnoreCollision(playerCollider,prefabInstance.GetComponent<Collider2D>());

            
           if(prefabInstance.TryGetComponent(out Rigidbody2D rb))
           {
               rb.velocity =rb.transform.up *projectileSpeed;
           }

        }
        
        //This is the code snippet that we are actually firing the projectile.
        [ServerRpc]
        private void SpawnProjectileServerRpc(Vector3 position, Vector3 direction)
        {
            if (wallet.TotalCoins.Value < costToFire) { return; }

            wallet.SpendCoins(costToFire);

            var prefabInstance = Instantiate(serverProjectilePrefab,
                position, 
                Quaternion.identity);
            prefabInstance.transform.up = direction;
            
            Physics2D.IgnoreCollision(playerCollider,prefabInstance.GetComponent<Collider2D>());

            if (prefabInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damageOnContact))
            {
                damageOnContact.SetOwner(OwnerClientId);
            }
            
            
            if(prefabInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity =rb.transform.up *projectileSpeed;
            }
            
            
            SpawnProjectileClientRpc(position, direction);
        }

        //This is the code for others too see that our projectile is fired.
        [ClientRpc]
        private void SpawnProjectileClientRpc(Vector3 position, Vector3 direction)
        {
            if (IsOwner) return;
            SpawnProjectile(position,direction);
        }
        
        
    }
}
