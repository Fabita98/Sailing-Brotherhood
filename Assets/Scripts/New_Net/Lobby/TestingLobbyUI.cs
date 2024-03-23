using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class TestingLobbyUI : NetworkBehaviour {
    public static TestingLobbyUI Instance { get; private set; }

    [SerializeField] private Button createGameButton;    
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button quitGame;

    private void Awake()
    {
        Instance = this;

        createGameButton.onClick.AddListener(() =>
        {
            SailingBrotheroodLobby.Instance.CreateLobby("Prima lobby", false);
        });                

        joinGameButton.onClick.AddListener(() =>
        {
            SailingBrotheroodLobby.Instance.QuickJoin();
        });

        readyButton.onClick.AddListener(() =>
        {
            SailingBrotheroodLobby.Instance.SetPlayerReady();
        });

        quitGame.onClick.AddListener(() =>
        {
            SailingBrotheroodLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Application.Quit();
        });
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}