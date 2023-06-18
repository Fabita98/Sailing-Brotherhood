using Crest;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class AnchorTriggerNet : NetworkBehaviour
{
    public GameObject anchor;
    public Button button;
    public Text textButton;
    public int cont = 0, value=0;
    public int necessaryPress = 10;
    private int child = 41;
    private float movementSpeed = 0.1f;
    public GameObject ship;
    private bool entered;
    private PlayerMovementNet playerMovement;
    private bool lockMovement;


    public bool start;
    
    private bool interact;
    public GameObject anchorUp;
    public AudioSource chainSound;
    private bool isplaying=false;

    private void Start()
    {
        entered = false;
        lockMovement = false;
        start = false;
        interact = false;
    }

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnSpawnedShip?.Invoke(this, EventArgs.Empty);
    }*/

    private void OnTriggerEnter(Collider other)
    {
        
        //Verifico se ha schiacciato spazio 10 volte per vedere se l'ancora e sollevata
        if (cont < necessaryPress)
        {
            // Verifico se e entrato un GameObject di tipo player
            if (other.tag == "Player" )
            {
                playerMovement = other.GetComponent<PlayerMovementNet>();
                //attivo il bottone che dice "premi spazio per salire l'ancora"
                textButton.text = "Press E to interact";
                if (playerMovement.IsLocalPlayer)
                {
                    button.gameObject.SetActive(true);
                    enableOutline();
                }
                //la variabile booleana=true indica che il giocatore e dentro il cilindro
                entered = true;
            }
            //Se non � vicino all'ancora allora non lo vede
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Se esce disattivo il bottone e la variabile entered e falsa
        if (other.tag == "Player")
        {     
            entered = false;
            chainSound.Pause();
            isplaying = false;

            if (anchor != null)
            {
                if (playerMovement.IsLocalPlayer)
                {
                    button.gameObject.SetActive(false);
                    disableOutline();
                }
            }

            playerMovement = null;
        }
        
    }

    private void Update()
    {
        if (entered == true) {

            if (interact == false)
            {
                chainSound.Pause();
                isplaying = false;
                textButton.text = "Press space to pull anchor";
            }
            textButton.text = "Space key pressed " + cont + " times";
            print("Space key pressed " + cont + " times");
            if (start == false)
            {
                if (Input.GetKeyDown("space"))
                {
                    if (!isplaying)
                    {
                        chainSound.Play();
                        isplaying = true;
                    }
                    interact = true;
                    //cont tiene conto del numero di volte che e stato premuto spazio
                    cont++;
                    //value = cont;
                    //if (IsClient) LiftSingleServerRPC();
                    //else { LiftSingleClientRPC(); }
                    
                    anchor.transform.position = anchor.transform.position + new Vector3(0, movementSpeed, 0);
                    if (cont % 3 == 0)
                    {
                        //anchor.transform.GetChild(child).gameObject.GetComponent<Outline>().enabled = false;
                        //Destroy(anchor.transform.GetChild(child).gameObject);
                        anchor.transform.GetChild(child).gameObject.SetActive(false);
                        child--;
                    }
                    //Se arriviamo a 10 la nave deve iniziare a muoversi e il bottone si disattiva
                    if (cont == necessaryPress)
                    {
                        if (isplaying)
                        {
                            chainSound.Pause();
                            isplaying = false;
                        }
                        //Destroy(anchor);
                                               
                        if (start == false)
                        {
                            start = true;
                            if (IsClient) LiftAnchorServerRPC();
                            else { LiftAnchorClientRPC(); }
                        }
                    }
                }
            }
            else
            {
                cont = 10;
                anchor.SetActive(false);
                anchorUp.SetActive(true);
                button.gameObject.SetActive(false);
                entered = false;
            }
        }    
    }

    public void enableOutline()
    {    
            Outline outline = anchor.GetComponent<Outline>();
            outline.enabled = true;      
    }
    public void disableOutline()
    {   
            Outline outline = anchor.GetComponent<Outline>();
            outline.enabled = false;        
    }

    [ServerRpc(RequireOwnership = false)]
    private void LiftAnchorServerRPC()
    {
        LiftAnchorClientRPC();
    }
    [ClientRpc]
    private void LiftAnchorClientRPC()
    {
        start = true;
        cont = 10;
        Debug.Log("ho sollevato l'ancora");
    }
    //[ServerRpc(RequireOwnership = false)]
    //private void LiftSingleServerRPC()
    //{
    //    LiftSingleClientRPC();
    //}
    //[ClientRpc]
    //private void LiftSingleClientRPC()
    //{
    //    cont = value;
    //}

}