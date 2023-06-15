//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class TestingLobbyUI : MonoBehaviour {

//    [SerializeField] private Button createGameButton;
//    [SerializeField] private Button joinGameButton;
//    [SerializeField] private Button readyButton;

//    private void Awake() {
//        createGameButton.onClick.AddListener(() => {
//            LobbyTest.Instance.CreateLobby();
//            //Loader.LoadNetwork(Loader.Scene.Race_Net_Test);
//        });

//        joinGameButton.onClick.AddListener(() => {
//            LobbyTest.Instance.QuickJoinLobby();
//        });

//        readyButton.onClick.AddListener(() => {
//            LobbyTest.Instance.SetPlayerReady();
//        });
//    }

//}