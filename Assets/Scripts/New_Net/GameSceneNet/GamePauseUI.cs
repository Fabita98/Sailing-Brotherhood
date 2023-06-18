using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject menu;
    private bool pressed = false;

    private void Awake()
    {        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && pressed == false)
        {
            Cursor.visible=true;
            Cursor.lockState = CursorLockMode.None;
            pressed = true;
            Show();
        }
        else if (Input.GetKeyDown(KeyCode.P) && pressed== true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pressed = false;
            Hide();
        }        
    }
    

    private void Start()
    {
        SailingBrotherhoodManager.Instance.OnLocalGamePaused += SailingBrotherhoodManager_OnLocalGamePaused;
        SailingBrotherhoodManager.Instance.OnLocalGameUnpaused += SailingBrotherhoodManager_OnLocalGameUnpaused;

        Hide();
    }

    private void SailingBrotherhoodManager_OnLocalGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void SailingBrotherhoodManager_OnLocalGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    public void Show()
    {
        menu.SetActive(true);
    }

    public void Hide()
    {
        menu.SetActive(false);
    }

    public void QuitGame()
    {
        if (PlayerMovementNet.LocalInstance.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
            Loader.LoadNetwork(Loader.Scene.Lobby);
        }

        else if (PlayerMovementNet.LocalInstance.IsClient)
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