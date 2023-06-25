using Crest;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class WheelTriggerNet : NetworkBehaviour
{
    private int cont;
    private bool lockMovement;
    private PlayerMovementNet playerMovement;

    private GameObject player;
    public GameObject wheel;
    public Transform wheelPos;


    private bool anyinput, inputleft, inputright, externaldriver;
    private bool rotateRight;
    private bool rotateLeft;

    private BoatProbes boatProbes;
    private GameObject ship;

    public float rotationSpeed;

    public float maxTurnBias;

    public Button button;
    public Text textButton;

    private bool wheelOccupied;
    private int contPlayers;

    public float _turningHeel = 0.2f;
    public float _turnPower = 0.2f;

    public Rigidbody _rb;
    public GameObject navigationSpot;

    public float sideways;
    public Vector3 rotVec;
    public AudioSource footsteps;

    private void Start()
    {
        // Ottieni il riferimento a boatProbes partendo da wheel e prendendo i padri
        GameObject wheelArea = wheel.transform.parent.gameObject;
        GameObject shipComponent = wheelArea.transform.parent.gameObject;
        ship = shipComponent.transform.parent.gameObject;
        boatProbes = ship.GetComponent<BoatProbes>();
        wheelOccupied = false;
        contPlayers = 0;

    }

    private void Update()
    {
        if (cont == 1)
        {
            if(wheelOccupied == true) { player.transform.position = wheelPos.transform.position;  }
            if (wheelOccupied == false && Input.GetKeyDown(KeyCode.E) && lockMovement == false)
            {
                wheelOccupied = true;
                if (IsClient) WheelOccupiedServerRPC();
                else { WheelOccupiedClientRPC(); }

                player.transform.rotation = wheelPos.transform.rotation * Quaternion.Euler(0, 0, 0);
                

                textButton.text = "Press A or D\nto rotate the wheel";
                float distance = 2;
                player.transform.position = wheelPos.transform.position;     /*wheel.transform.position - wheel.transform.forward * distance;*/
                
                navigationSpot.SetActive(true);
                //Qui si ferma la visuale
                if (playerMovement != null)
                {
                    //playerMovement.enabled = false;
                    playerMovement.LockMovement();

                    // Disabilita lo script PlayerMovement
                }
                lockMovement = true;
                if (!IsHost)
                {
                    externaldriver = true;
                    externalServerRPC();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && lockMovement == true)
            {
                wheelOccupied = false;
                if (IsClient) WheelNotOccupiedServerRPC();
                else { WheelNotOccupiedClientRPC(); }

                navigationSpot.SetActive(false);
                //Qui si sblocca la visuale e puo muoversi nuovamente
                textButton.text = "Press E to use the wheel";
                if (playerMovement != null)
                {
                    //playerMovement.enabled = true; // Disabilita lo script PlayerMovement
                    playerMovement.UnlockMovement();

                }
                lockMovement = false;
                if (!IsHost) {externaldriver = false; NOexternalServerRPC();
            }
            }
            /*
            if (lockMovement == true)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    rotateLeft = true;
                    if (IsClient) RotateLeftServerRPC();
                    else { RotateLeftClientRPC(); }
                    rotateRight = false;
                    if (IsClient) NotRotateRightServerRPC();
                    else { NotRotateRightClientRPC(); }
                }

                else if (Input.GetKey(KeyCode.D))
                {
                    rotateLeft = false;
                    if (IsClient) NotRotateLeftServerRPC();
                    else { NotRotateLeftClientRPC(); }
                    rotateRight = true;
                    if (IsClient) RotateRightServerRPC();
                    else { RotateRightClientRPC(); }
                }
                else
                {
                    rotateLeft = false;
                    if (IsClient) NotRotateLeftServerRPC();
                    else { NotRotateLeftClientRPC(); }
                    rotateRight = false;
                    if (IsClient) NotRotateRightServerRPC();
                    else { NotRotateRightClientRPC(); }
                }
            }*/
        }
    }

    private void FixedUpdate()
    {

        sideways = boatProbes.getTurnBias();

        if (!IsHost)
        {
            
            if (Input.GetKey(KeyCode.A))
            {
                LeftServerRPC();
                if (externaldriver) wheel.transform.Rotate(Vector3.forward, 90f * Time.fixedDeltaTime);
            }
            else if (Input.GetKey(KeyCode.D)) { 
                rightServerRPC();
                if (externaldriver) wheel.transform.Rotate(Vector3.back, 90f * Time.fixedDeltaTime);
            }

        else nullServerRPC();

            
        }
        else if (anyinput && externaldriver == true)
        {
            
            if (inputleft)
            {
                wheel.transform.Rotate(Vector3.forward, 90f * Time.fixedDeltaTime);
                sideways += -0.3f;
            }

            if (inputright)
            {
                wheel.transform.Rotate(Vector3.back, 90f * Time.fixedDeltaTime);
                sideways += 0.3f;
            }
            rotVec = transform.up + _turningHeel * transform.forward;
            
            _rb.AddTorque(rotVec * _turnPower * sideways, ForceMode.Acceleration);
        }
        else if (lockMovement == true)
        {
           
            if (Input.GetKey(KeyCode.A))
            {
                wheel.transform.Rotate(Vector3.forward, 90f * Time.fixedDeltaTime);
                sideways += -0.3f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                wheel.transform.Rotate(Vector3.back, 90f * Time.fixedDeltaTime);
                sideways += 0.3f;
            }
            rotVec = transform.up + _turningHeel * transform.forward;
            
            _rb.AddTorque(rotVec * _turnPower * sideways, ForceMode.Acceleration);

        }


    }


    /*private void FixedUpdate()
    {
        if (rotateLeft&&boatProbes.getTurnBias()>-maxTurnBias )
        {
            // Ruota il timone verso sinistra in ogni frame di FixedUpdate
            wheel.transform.Rotate(Vector3.forward, 90f * Time.fixedDeltaTime);
            //boatProbes.setTurnBias(boatProbes.getTurnBias()+1);
            float turnBiasIncrement = 1.0f; // Incremento desiderato

            // Aggiorna gradualmente il valore di turnBias utilizzando Lerp
            float currentTurnBias = boatProbes.getTurnBias();
            float targetTurnBias = currentTurnBias - turnBiasIncrement;
            float newTurnBias = Mathf.Lerp(currentTurnBias, targetTurnBias, rotationSpeed * Time.fixedDeltaTime);

            // Imposta il nuovo valore di turnBias
            boatProbes.setTurnBias(newTurnBias);
        }
        else if (rotateRight && boatProbes.getTurnBias() < maxTurnBias)
        {
            // Ruota il timone verso destra in ogni frame di FixedUpdate
            wheel.transform.Rotate(Vector3.back, 90f * Time.fixedDeltaTime);
            float turnBiasIncrement = 1.0f; // Incremento desiderato

            // Aggiorna gradualmente il valore di turnBias utilizzando Lerp
            float currentTurnBias = boatProbes.getTurnBias();
            float targetTurnBias = currentTurnBias + turnBiasIncrement;
            float newTurnBias = Mathf.Lerp(currentTurnBias, targetTurnBias, rotationSpeed * Time.fixedDeltaTime);

            // Imposta il nuovo valore di turnBias
            boatProbes.setTurnBias(newTurnBias);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            contPlayers += 1;
            cont = 1;
            //Qui prendiamo lo script del movimento del pirata che ha triggerato i cannoni
            player = other.gameObject;
            playerMovement = other.GetComponent<PlayerMovementNet>();
            if (playerMovement.IsLocalPlayer)
            {
                enableOutline();
                button.gameObject.SetActive(true);
            }
            footsteps = player.GetComponentInChildren<AudioSource>();
            footsteps.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            contPlayers -= 1;
            if (contPlayers == 0)
            {
                wheelOccupied = false;
                if (IsClient) WheelNotOccupiedServerRPC();
                else { WheelNotOccupiedClientRPC(); }
                navigationSpot.SetActive(false);
            }
            textButton.text = "Press E to use the wheel";
            if (playerMovement != null)
            {
                //playerMovement.enabled = true; // Disabilita lo script PlayerMovement
                playerMovement.UnlockMovement();

            }
            lockMovement = false;
            player = null;
            cont = 0;
            if (playerMovement.IsLocalPlayer) { 
                disableOutline();
                button.gameObject.SetActive(false);
            }
            playerMovement = null;
            footsteps.gameObject.SetActive(true);
            footsteps = null;
            
        }
    }

    public void enableOutline()
    {
        Outline outline = wheel.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = wheel.GetComponent<Outline>();
        outline.enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveSidewaysServerRPC()
    {
        RemoveSidewaysClientRPC();
    }

    [ClientRpc]
    private void RemoveSidewaysClientRPC()
    {
        sideways += -0.3f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddSidewaysServerRPC()
    {
        AddSidewaysClientRPC();
    }

    [ClientRpc]
    private void AddSidewaysClientRPC()
    {
        sideways += 0.3f;
    }

    [ServerRpc(RequireOwnership = false)]
    private void WheelOccupiedServerRPC()
    {
        WheelOccupiedClientRPC();
    }

    [ClientRpc]
    private void WheelOccupiedClientRPC()
    {
        wheelOccupied = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void WheelNotOccupiedServerRPC()
    {
        WheelNotOccupiedClientRPC();
    }

    [ClientRpc]
    private void WheelNotOccupiedClientRPC()
    {
        wheelOccupied = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RotateLeftServerRPC()
    {
        RotateLeftClientRPC();
    }

    [ClientRpc]
    private void RotateLeftClientRPC()
    {
        rotateLeft = true; ;
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotRotateLeftServerRPC()
    {
        NotRotateLeftClientRPC();
    }

    [ClientRpc]
    private void NotRotateLeftClientRPC()
    {
        rotateLeft = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RotateRightServerRPC()
    {
        RotateRightClientRPC();
    }

    [ClientRpc]
    private void RotateRightClientRPC()
    {
        rotateRight = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotRotateRightServerRPC()
    {
        NotRotateRightClientRPC();
    }

    [ClientRpc]
    private void NotRotateRightClientRPC()
    {
        rotateRight = false;
    }

    //max____________________________________________________________________________

    [ServerRpc(RequireOwnership = false)]
    private void LeftServerRPC()
    {
        LeftClientRPC();
    }

    [ClientRpc]
    private void LeftClientRPC()
    {
        anyinput = true; inputleft = true; inputright = false;
    }
    [ServerRpc(RequireOwnership = false)]
    private void rightServerRPC()
    {
        rightClientRPC();
    }

    [ClientRpc]
    private void rightClientRPC()
    {
        anyinput = true; inputleft = false; inputright = true;
    }

    

    [ServerRpc(RequireOwnership = false)]
    private void nullServerRPC()
    {
        nullClientRPC();
    }

    [ClientRpc]
    private void nullClientRPC()
    {
        anyinput = false; inputleft = false; inputright = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void externalServerRPC()
    {
        externalClientRPC();
    }

    [ClientRpc]
    private void externalClientRPC()
    {
        externaldriver = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void NOexternalServerRPC()
    {
        NOexternalClientRPC();
    }

    [ClientRpc]
    private void NOexternalClientRPC()
    {
        externaldriver = false;
    }



    


}