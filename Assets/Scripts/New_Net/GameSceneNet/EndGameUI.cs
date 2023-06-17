using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : NetworkBehaviour
{
    public static EndGameUI Instance { get; private set; }


    [SerializeField] private Button mainMenu;

    private void Awake() {
        Instance = this;        

        mainMenu.onClick.AddListener(() => {
        Loader.Load(Loader.Scene.Lobby);
    }); }
    
}
