using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;

         private async void Start()
         {
             DontDestroyOnLoad(gameObject);
             //check if it is server with method
            await LaunchMode(SystemInfo.graphicsDeviceType ==UnityEngine.Rendering.GraphicsDeviceType.Null);
         }

         private async Task LaunchMode(bool isDedicatedServer)
         {

             if (isDedicatedServer)
             {
                 //do server stuffx
             }
             else
             {
                 HostSingleton hostSingleton = Instantiate(hostPrefab);
                 hostSingleton.CreateHost();
                 ClientSingleton clientSingleton= Instantiate(clientPrefab);
                 bool authenticated =await clientSingleton.CreateClient();
                 
                   if (authenticated)
                   {
                       clientSingleton.ClientGameManager.OpenMenu();
                   }
             }


         }
     
       
}
