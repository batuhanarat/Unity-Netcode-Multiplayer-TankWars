using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class CoinBase : NetworkBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;
    protected bool isCollected;
    protected int coinValue;

    public abstract int Collect();

    public void SetValue(int value)
    {
        coinValue = value;
    }

    protected void Show(bool show)
    {
        spriteRenderer.enabled = show;
    }






}