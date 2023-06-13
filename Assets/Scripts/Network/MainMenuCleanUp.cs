using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour {


    private void Awake() {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        //if (SailingBrotherhoodMultiplayer.Instance != null) {
        //    Destroy(SailingBrotherhoodMultiplayer.Instance.gameObject);
        //}

        if (SailingBrotheroodLobby.Instance != null) {
            Destroy(SailingBrotheroodLobby.Instance.gameObject);
        }
    }

}