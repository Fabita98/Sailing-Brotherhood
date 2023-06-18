using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDisconnectUI : NetworkBehaviour
{
    public static PlayerDisconnectUI Instance { get; private set; }

    [SerializeField] private Text returnToLobby;    

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        Show();        
        Invoke(nameof(ShutdownInvoke), 2);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShutdownInvoke()
    {
        if (IsHost)
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (SailingBrotheroodLobby.Instance != null)
            {
                Destroy(SailingBrotheroodLobby.Instance.gameObject);
            }
            NetworkManager.Singleton.Shutdown();
            //Loader.LoadNetwork(Loader.Scene.Lobby);
            Loader.Load(Loader.Scene.Lobby);
        }
        else if (IsClient)
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(NetworkManager.Singleton.gameObject);
            }
            if (SailingBrotheroodLobby.Instance != null)
            {
                Destroy(SailingBrotheroodLobby.Instance.gameObject);
            }
            Loader.Load(Loader.Scene.Lobby);
        }
    }

}