using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


public class RepairTrigger : MonoBehaviour
{
    public GameObject repair;
    private bool entered;

    private GameObject player;

    public Button button;
    public Text textButton;

    public GameObject ship;
    private Health_and_Speed_Manager health;

    public float holdTimeRequired = 8f;
    private float holdTime = 0f;

    public AudioSource repairsound;
    private bool isplaying = false;

    // Start is called before the first frame update
    void Start()
    {
        health = ship.GetComponent<Health_and_Speed_Manager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (entered == true)
        {
            if (health.health == 100) { 
            textButton.text = "Health: " + health.health;
                
            }
            else
            {
                textButton.text = "Health: " + health.health +"\n"+"Press R to repair";
            }
            if (health.health<100&&Input.GetKey(KeyCode.R))
            {
                holdTime += Time.deltaTime;
                Debug.Log("Holdtime: " + holdTime);
                float waitingTime = holdTimeRequired - holdTime;
                string formattedValue = waitingTime.ToString("F2");
                textButton.text = "" + formattedValue;
                if (!isplaying)
                {
                    repairsound.Play();
                    isplaying = true;
                }
                if (holdTime >= holdTimeRequired)
                {
                    health.health += 10f;
                    holdTime = 0;
                    if (health.health == 100)
                    {
                        textButton.text = "Health: " + health.health;
                    }
                    else
                    {
                        textButton.text = "Health: " + health.health + "\n" + "Press R to repair";
                    }                                                          
                }
            } else { repairsound.Pause();
                isplaying = false;
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

            //attivo il bottone che dice "premi E per interagire"
            button.gameObject.SetActive(true);
            enableOutline();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = null;
            //Se esce disattivo il bottone e la variabile entered e falsa
            button.gameObject.SetActive(false);
            entered = false;
            disableOutline();
        }
    }
    public void enableOutline()
    {
        Outline outline = repair.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = repair.GetComponent<Outline>();
        outline.enabled = false;
    }
}
