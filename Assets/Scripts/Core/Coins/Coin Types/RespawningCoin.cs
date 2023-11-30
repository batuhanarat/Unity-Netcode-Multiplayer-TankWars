using System;
using UnityEngine;

namespace Core.Coins.Coin_Types
{
    public class RespawningCoin : CoinBase
    {
        public event Action<RespawningCoin> OnCollected;
        private Vector3 previousPosition;

        private void Update()
        {
            if (previousPosition != transform.position)
            {
                Show(true);
            }

            previousPosition = transform.position;
        }

        /*For the sake of client smoothness experience,
         * when client is collided, directly enable the Sprite Renderer of the coin.
         * For the server, we are invoking onCollected which is listening by CoinSpawner
         * in order to respawn the coin.
         */
        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (isCollected) { return 0; }

            isCollected = true;
            OnCollected?.Invoke(this);

            return coinValue;
        }
        public void Reset()
        {
            isCollected = false;
        }

    }
}
