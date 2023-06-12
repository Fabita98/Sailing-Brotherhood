using Crest;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelTrigger : MonoBehaviour
{
    private int cont;
    private bool lockMovement;
    private PlayerMovement playerMovement;

    private GameObject player;
    public GameObject wheel;

    private bool rotateRight;
    private bool rotateLeft;

    private BoatProbes boatProbes;
    private GameObject ship;

    public float rotationSpeed;

    public float maxTurnBias;

    public Button button;
    public Text textButton;

    private void Start()
    {
        // Ottieni il riferimento a boatProbes partendo da wheel e prendendo i padri
        GameObject wheelArea = wheel.transform.parent.gameObject;
        GameObject shipComponent = wheelArea.transform.parent.gameObject;
        ship = shipComponent.transform.parent.gameObject;
        boatProbes= ship.GetComponent<BoatProbes>();
    }

    private void Update()
    {
        if (cont == 1)
        {
            if (Input.GetKeyDown(KeyCode.E) && lockMovement == false)
            {
                player.transform.rotation = wheel.transform.rotation * Quaternion.Euler(0, 0, 0);

                textButton.text = "Press A or D\nto rotate the wheel";
                float distance = 2;
                player.transform.position = wheel.transform.position - wheel.transform.forward * distance;
                //Qui si ferma la visuale
                if (playerMovement != null)
                {
                    //playerMovement.enabled = false;
                    playerMovement.LockMovement();

                    // Disabilita lo script PlayerMovement
                }
                lockMovement = true;
            }
            else if (Input.GetKeyDown(KeyCode.E) && lockMovement == true)
            {
                //Qui si sblocca la visuale e puo muoversi nuovamente
                textButton.text = "Press E to interact";
                if (playerMovement != null)
                {
                    //playerMovement.enabled = true; // Disabilita lo script PlayerMovement
                    playerMovement.UnlockMovement();

                }
                lockMovement = false;
            }

            if (lockMovement == true)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    rotateLeft = true;
                    rotateRight = false;
                }             

                else if (Input.GetKey(KeyCode.D))
                {
                    rotateLeft = false;
                    rotateRight = true;
                }
                else 
                {
                    rotateLeft = false;
                    rotateRight = false;
                }
            }
        }

    }

    private void FixedUpdate()
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            button.gameObject.SetActive(true);
            cont = 1;
            //Qui prendiamo lo script del movimento del pirata che ha triggerato i cannoni
            player = other.gameObject;
            playerMovement = other.GetComponent<PlayerMovement>();
            enableOutline();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            textButton.text = "Press E to interact";
            if (playerMovement != null)
            {
                //playerMovement.enabled = true; // Disabilita lo script PlayerMovement
                playerMovement.UnlockMovement();

            }
            button.gameObject.SetActive(false);
            lockMovement = false;
            player = null;
            cont = 0;
            playerMovement = null;

            disableOutline();
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

}
