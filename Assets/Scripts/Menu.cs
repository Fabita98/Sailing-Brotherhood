using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private bool pressed;
    public GameObject menu,sure;
    public Button resume, quit;

    // Start is called before the first frame update
    void Start()
    {
        pressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)&& pressed==false)
        {
            Cursor.visible=true;
            Cursor.lockState = CursorLockMode.None;
            pressed = true;
            menu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.P) && pressed == true)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            pressed = false;
            menu.SetActive(false);
        }
        
    }

    public void resumeMatch()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu.SetActive(false);
        pressed = false;
    }

    public void quitMatch()
    {
        sure.SetActive(true);
    }

    public void yes()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void no()
    {
        sure.SetActive(false);
    }
}
