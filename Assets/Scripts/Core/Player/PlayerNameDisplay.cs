using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisplay : MonoBehaviour
{

    [SerializeField] private TankPlayer _tankPlayer;
    [SerializeField] private TMP_Text playerNameText;
    
    
    void Start()
    {
        HandlePlayerName(" ", _tankPlayer.playerName.Value);
        _tankPlayer.playerName.OnValueChanged += HandlePlayerName;

    }

    private void HandlePlayerName(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        playerNameText.text = newName.ToString();

    }

    private void OnDestroy()
    {
        _tankPlayer.playerName.OnValueChanged -= HandlePlayerName;

    }
}
