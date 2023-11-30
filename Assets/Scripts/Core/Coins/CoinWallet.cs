using Core.Combat;
using Unity.Netcode;
using UnityEngine;

namespace Core.Coins
{
    public class CoinWallet : NetworkBehaviour
    {

        [Header("References")]
        [SerializeField] private Health health;
        [SerializeField] private BountyCoin bountyCoinPrefab;


        [Header("Settings")] 
        [SerializeField] private float coinOffset = 3f;
        [SerializeField] private float bountyPercentage = 50f;
        [SerializeField] private int coinCount = 10;
        [SerializeField] private int minCoin = 5;
        [SerializeField] private LayerMask layerMask;
        
        
        private Collider2D[] coinBuffer = new Collider2D[1];
        private float coinRadius;

      

        //We are choosing Network Variable 
        public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            coinRadius =  bountyCoinPrefab.GetComponent<CircleCollider2D>().radius;

            if (IsServer)
            {
                health.OnDead += HandleDie;

            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                health.OnDead -= HandleDie;
            }
        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.TryGetComponent<CoinBase>(out CoinBase coin)) {return;} 
        
            int coinValue = coin.Collect();
            
            if (!IsServer) {return;}
            
            TotalCoins.Value += coinValue;
            Debug.Log(OwnerClientId +" Total coin: " + TotalCoins.Value);

            
        }

        public void SpendCoins(int spending)
        {
            TotalCoins.Value -= spending;
        }

        private void HandleDie(Health health)
        {
            int bountyValue = (int) (TotalCoins.Value * (bountyPercentage / 100f));
            int bountyCoinValue = bountyValue / coinCount;

            if (bountyCoinValue < minCoin) { return; }

            for (int i = 0; i < coinCount; i++)
            {
                   
                BountyCoin coinInstance = 
                    Instantiate(bountyCoinPrefab,
                        GetSpawnPoint(), 
                        Quaternion.identity);
            
                coinInstance.SetValue(bountyCoinValue);
                coinInstance.NetworkObject.Spawn();  
            }
           
            

        }
        
        private Vector2 GetSpawnPoint()
        {
            
            while (true)
            {
               
                Vector2 position = (Vector2)transform.position + (coinOffset * Random.insideUnitCircle);
              
                int numColliders = Physics2D.OverlapCircleNonAlloc(position, coinRadius, coinBuffer, layerMask);
           
                if (numColliders == 0) { return position; }
            }


        }
        
    }
}
