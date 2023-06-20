using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class BarrelTriggerNet : NetworkBehaviour
{
    public GameObject originalBarrel;
    public GameObject barrel;
    private bool entered;
    private bool enteredPlayer;
    private bool lockMovement;
    private PlayerMovementNet playerMovement;
    private GameObject player;
    public Button button;
    public Text textButton;
    public AudioSource barrelSplash;

    public int cont;
    // Start is called before the first frame update
    void Start()
    {
        lockMovement = false;
        cont = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (entered == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && cont == 0)
            {
                textButton.text = "Number of barrels: " + cont;
            }

            else if (Input.GetKeyDown(KeyCode.E) && cont > 0 && enteredPlayer == true)
            {
                //qua rilascia il barile
                if (IsClient) ReleaseBarrelServerRPC();
                else { ReleaseBarrelClientRPC(); }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {   //la variabile booleana=true indica che il giocatore e dentro il cilindro
            entered = true;
            //Qui prendiamo lo script del movimento del pirata che ha triggerato i cannoni
            playerMovement = other.GetComponent<PlayerMovementNet>();
            player = other.gameObject;
            //attivo il bottone che dice "premi E per interagire"
            if (playerMovement.IsLocalPlayer)
            {
                enteredPlayer = true;
                button.gameObject.SetActive(true);
                enableOutline();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {         
            player = null;
            //Se esce disattivo il bottone e la variabile entered e falsa
            if (playerMovement.IsLocalPlayer)
            {
                enteredPlayer = false;
                button.gameObject.SetActive(false);
                disableOutline();
            }
            entered = false;
            playerMovement = null;
        }
    }
    public void enableOutline()
    {
        Outline outline = originalBarrel.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = originalBarrel.GetComponent<Outline>();
        outline.enabled = false;
    }

    public void addBarrel(int number)
    {
        cont += number;
        textButton.text = "Number of barrels: " + cont;
    }

    public void releaseBarrel()
    {
        cont--;
        textButton.text = "Number of barrels: " + cont + "\n" + "Press E to release one barrel";
        //la distanza da cui deve spawnare il barile dietro la nave
        float spawnDistance = 20f;
        //l'altezza da cui deve partire(altrimenti parte sotto la barca)
        float spawnHeight = 0f;

        //posizione in cui spawnare la palla
        Vector3 spawnPosition = transform.position + -barrel.transform.forward * spawnDistance + Vector3.up * spawnHeight;
        //Instanziamento della palla
        GameObject barrel1 = Instantiate(barrel, spawnPosition, transform.rotation);
        barrel1.AddComponent<Rigidbody>();
        barrel1.AddComponent<BoatAlignNormal>();
        barrel1.tag = "Barrel";
        barrelSplash.PlayDelayed(1);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReleaseBarrelServerRPC()
    {
        ReleaseBarrelClientRPC();
    }

    [ClientRpc]
    public void ReleaseBarrelClientRPC()
    {
        releaseBarrel();
    }

}