using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class TestingLobbyUI : NetworkBehaviour {
    public static TestingLobbyUI Instance { get; private set; }

    [SerializeField] private Button createGameButton;
    //[SerializeField] private Button lobbyNameButton;
    //[SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button quitGame;

    private string lobbyName;

    private void Awake()
    {
        Instance = this;

        createGameButton.onClick.AddListener(() =>
        {
            LobbyTest.Instance.CreateLobby();
        });

        //Creare da comando finch� non si � finito il bottone input LobbyName
        //lobbyNameButton.onClick.AddListener(() =>
        //{
        //    UI_InputWindow.Show_Static("Lobby Name", lobbyName, "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ .,-", 20,
        //    () =>
        //    {
        //        // Cancel
        //    },
        //    (string lobbyName) =>
        //    {
        //        this.lobbyName = lobbyName;
        //        UpdateText();
        //    });
        //});        

        joinGameButton.onClick.AddListener(() =>
        {
            LobbyTest.Instance.QuickJoinLobby();
        });

        readyButton.onClick.AddListener(() =>
        {
            LobbyTest.Instance.SetPlayerReady();
        });

        quitGame.onClick.AddListener(() =>
        {
            LobbyTest.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Application.Quit();
        });
    }
    //private void UpdateText()
    //{
    //    lobbyNameText.text = lobbyName;
    //}    
}