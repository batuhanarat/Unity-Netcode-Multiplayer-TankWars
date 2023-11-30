using Core.Coins.Coin_Types;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Coins
{
    public class CoinSpawner : NetworkBehaviour
    {

        [SerializeField] private RespawningCoin coinPrefab;
        [SerializeField] private Vector2 xSpawnRange;
        [SerializeField] private Vector2 ySpawnRange;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private int maxCoins = 50;
        [SerializeField] private int coinValue = 10;


        private Collider2D[] coinBuffer = new Collider2D[1];
        private float coinRadius;


        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
        
            coinRadius =  coinPrefab.GetComponent<CircleCollider2D>().radius;
            for (int i = 0; i < maxCoins; i++)
            {
                SpawnCoin();
            }

        }

       /*We need to call Spawn method in order to make our coinInstance a Networked Object.
        * Yes it has NetworkObject and NetworkTransform component attached but it is not enough.
        * If we dragged and dropped prefab, Unity would handle its being networked, but if
        * we are going to instantiate by code, just instantiating wouldnt enough,
        * we should also add it to network prefabs and called its spawn method.
        */

        private void SpawnCoin()
        {
            RespawningCoin coinInstance =  Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(coinValue);
            coinInstance.GetComponent<NetworkObject>().Spawn();
            coinInstance.OnCollected += HandleCoinCollected;

        }
        
        private void HandleCoinCollected(RespawningCoin coin)
        {
            coin.transform.position = GetSpawnPoint();
            coin.Reset();
        }

        /*
         * Summary: It calculates random position with given ranges.
         * Unity create a circle with given position and radius, 
         *if it is collides something that is layer allowed by the LayerMask,
         *it adds the collider to the coinBuffer array, 
         *and returns the total collider size that is on circle. 
         * While it is not colliding with anything,
         *we are keep searching for suitable position.
         * Normally this approach can be create bottleneck if it is in a scene that
         *it is hard to suitable place, but in our game we know there are plenty of spaces
         *and we are keeping maxCoin size reasonable so it is Ok.
         */
        private Vector2 GetSpawnPoint()
        {
            float x = 0;
            float y = 0;


            while (true)
            {
                x = Random.Range(xSpawnRange.x, xSpawnRange.y);
                y = Random.Range(ySpawnRange.x, ySpawnRange.y);
                Vector2 position = new Vector2(x, y);
              
                int numColliders = Physics2D.OverlapCircleNonAlloc(position, coinRadius, coinBuffer, layerMask);
           
                if (numColliders == 0) { return position; }
            }


        }
    }
}
