using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField] private Color myColor;
    private FixedString32Bytes playerName;

    public ulong ClientId{ get; private set; }
    public int Coins { get; private set;  }
    

    public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins)
    {
        this.ClientId = clientId;
        this.playerName = playerName;
        UpdateCoins(coins);
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            leaderboardText.color = myColor;
        }

    }


    public void UpdateCoins(int coins)
    {
        this.Coins = coins;
        UpdateText();
    }

    public void UpdateText()
    {
        leaderboardText.text = $"{gameObject.transform.GetSiblingIndex()+1}.{playerName} ({Coins})";
    }
}
