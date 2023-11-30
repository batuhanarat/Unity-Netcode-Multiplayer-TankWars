using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyCoin : CoinBase
{
    
 
    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (isCollected) { return 0; }

        isCollected = true; 
        Destroy(gameObject);
        return coinValue;    
    }
}
