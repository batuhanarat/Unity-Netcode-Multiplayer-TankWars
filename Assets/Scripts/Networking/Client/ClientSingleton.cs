using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{

    private static ClientSingleton instance;
    public ClientGameManager ClientGameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get{
            if (instance != null) { return instance; }
           
            instance = FindObjectOfType<ClientSingleton>();
            return instance;
            
        }
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
    }

    public async Task<bool> CreateClient()
    {
        ClientGameManager = new ClientGameManager();

        return await ClientGameManager.InitAsync();
    }

    private void OnDestroy()
    {
        ClientGameManager?.Dispose();
    }
}
