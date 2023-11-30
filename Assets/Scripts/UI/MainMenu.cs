using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private TMP_InputField joinCode;
    
    
   public async void StartHost()
   {
       await HostSingleton.Instance.HostGameManager.StartHostAsync();
   }
   
   public async void StartClient()
   {
       await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCode.text);
   }

   
}
