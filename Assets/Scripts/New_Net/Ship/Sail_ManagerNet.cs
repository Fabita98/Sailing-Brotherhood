using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Sail_ManagerNet : NetworkBehaviour
{
    public GameObject sail;
    public GameObject belongingShip;
    Health_and_Speed_ManagerNet hs;
    bool entered = false, enteredPlayer, change=false;
    int count = 0;
    public GameObject mast;
    public AudioSource ropesound;

    private GameObject player;
    private PlayerMovementNet playerMovement;
    // need to be liked to 
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        hs = belongingShip.GetComponent<Health_and_Speed_ManagerNet>();
        if (entered)
        {
            if (Input.GetKeyDown("e") && sail.activeSelf && enteredPlayer==true)
            {
                if (IsClient) UseSailServerRPC();
                else
                {
                    UseSailClientRPC();
                    hs.maxspeed -= 6;
                    change = false;
                }
                ropesound.Play();
            }
            else if (Input.GetKeyDown("e") && sail.activeSelf == false && enteredPlayer==true)
            {
                if (IsClient) UseSailServerRPC(); else 
                {
                    UseSailClientRPC();
                    hs.maxspeed += 6;
                    change = false;
                }
                ropesound.Play();
            }
        }
        
        
        if (IsServer && change)
        {
            if (sail.activeSelf)
            {
                hs.maxspeed += 6;
                change = false;
            }
            else if (!sail.activeSelf)
            {
                hs.maxspeed -= 6;
                change = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { count++; entered = true;
            player = other.gameObject;
            playerMovement = other.GetComponent<PlayerMovementNet>();
            //attivo il bottone che dice "premi E per interagire"
            if (playerMovement.IsLocalPlayer)
            {
                enteredPlayer = true;
                enableOutline();
            }
        }
   
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player") { count--; if (count == 0)
            {
                entered = false;
                if (playerMovement.IsLocalPlayer)
                {
                    enteredPlayer = false;
                    disableOutline();
                }
                player = null;
                playerMovement = null;
            }
        }
        
    }
    public void enableOutline()
    {
        Outline outline = mast.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = mast.GetComponent<Outline>();
        outline.enabled = false;
    }
    [ServerRpc(RequireOwnership = false)]
    private void UseSailServerRPC()
    {
        UseSailClientRPC();
    }
    [ClientRpc]
    private void UseSailClientRPC()
    {
        change = true;
        sail.SetActive(!sail.activeSelf);
        
        
    }
}
