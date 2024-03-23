using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite Jack_Sparrow;
    [SerializeField] private Sprite Davy_Jones;
    [SerializeField] private Sprite Blackbeard;


    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManager.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManager.PlayerCharacter.Jack_Sparrow:   return Jack_Sparrow;
            case LobbyManager.PlayerCharacter.Davy_Jones:    return Davy_Jones;
            case LobbyManager.PlayerCharacter.Blackbeard:   return Blackbeard;
        }
    }

}