using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : NetworkBehaviour
{
    public static EndGameUI Instance { get; private set; }


    [SerializeField] private Button mainMenuLost;
    [SerializeField] private Button mainMenuWon;

    private void Awake() 
    {
        Instance = this;

        mainMenuLost.onClick.AddListener(() => {
        Loader.Load(Loader.Scene.MainTitle); });
        mainMenuWon.onClick.AddListener(() => {
        Loader.Load(Loader.Scene.MainTitle); }); 
    }
    
}
