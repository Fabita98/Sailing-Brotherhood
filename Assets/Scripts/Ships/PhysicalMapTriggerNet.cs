using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PhysicalMapTriggerNet : MonoBehaviour
{
    public GameObject map;

    private bool entered;
    private bool enteredPlayer;
    private GameObject player;

    private PlayerMovementNet playerMovement;

    public Button button;
    public Text textButton;

    private bool lockMovement;

    public GameObject arrow;
    public AudioSource mapSound;
    public GameObject x_Win;

    // Start is called before the first frame update
    void Start()
    {
        lockMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (entered == true)
        {
            if (Input.GetKeyDown(KeyCode.E)&&lockMovement==false && enteredPlayer==true)
            {
                mapSound.Play();
                arrow.gameObject.SetActive(true);
                x_Win.gameObject.SetActive(true);
                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = true;
                player.GetComponentInChildren<FirstPersonCamera>().lockVerticalRotation = true;
                playerMovement.LockMovement();
                //player.GetComponentInChildren<FirstPersonCamera>(). = true;
                lockMovement = true;
                Canvas canvasPlayer = player.transform.Find("Canvas").GetComponent<Canvas>();
                Button mapButton = canvasPlayer.transform.Find("MapButton").GetComponent<Button>();
                mapButton.gameObject.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.E) && lockMovement == true && enteredPlayer == true)
            {

                arrow.gameObject.SetActive(false);
                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = false;
                player.GetComponentInChildren<FirstPersonCamera>().lockVerticalRotation = false;
                playerMovement.UnlockMovement();
                lockMovement = false;
                Canvas canvasPlayer = player.transform.Find("Canvas").GetComponent<Canvas>();
                Button mapButton = canvasPlayer.transform.Find("MapButton").GetComponent<Button>();
                mapButton.gameObject.SetActive(false);
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {   //la variabile booleana=true indica che il giocatore e dentro il cilindro
            entered = true;
            //Qui prendiamo lo script del movimento del pirata che ha triggerato i cannoni      
            player = other.gameObject;
            playerMovement = other.GetComponent<PlayerMovementNet>();

            if (playerMovement.IsLocalPlayer)
            {
                enteredPlayer = true;
                enableOutline();
                //attivo il bottone che dice "premi E per interagire"
                button.gameObject.SetActive(true);
            }         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.IsLocalPlayer)
            {
                enteredPlayer = false;
                disableOutline();
                //attivo il bottone che dice "premi E per interagire"
                button.gameObject.SetActive(false);
            }
            arrow.gameObject.SetActive(false);
            if (player != null){ 
            player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = false;
            player.GetComponentInChildren<FirstPersonCamera>().lockVerticalRotation = false;
            }
            playerMovement.UnlockMovement();
            lockMovement = false;
            Canvas canvasPlayer = player.transform.Find("Canvas").GetComponent<Canvas>();
            Button mapButton = canvasPlayer.transform.Find("MapButton").GetComponent<Button>();
            mapButton.gameObject.SetActive(false);
            player = null;
            playerMovement = null;
            //Se esce disattivo il bottone e la variabile entered e falsa
            entered = false;            
        }
    }
    public void enableOutline()
    {
        Outline outline = map.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = map.GetComponent<Outline>();
        outline.enabled = false;
    }
}
