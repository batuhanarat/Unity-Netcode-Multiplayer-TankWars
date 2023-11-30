using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{

    private static HostSingleton instance;
    public HostGameManager HostGameManager { get; private set; }

    public static HostSingleton Instance
    {
        get{
            if (instance != null) { return instance; }
           
            instance = FindObjectOfType<HostSingleton>();
            return instance;
            
        }
    }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
    }

    public async void CreateHost()
    {
        HostGameManager = new HostGameManager();

    }

    public void OnDestroy()
    {
        HostGameManager?.Dispose();
    }
}
