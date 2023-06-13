using Crest;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class AnchorTrigger : MonoBehaviour
{
    public GameObject anchor;
    public Button button;
    public Text textButton;
    public int cont = 0;
    public int necessaryPress;
    private int child = 41;
    private float movementSpeed = 0.1f;
    public GameObject ship;
    private bool entered;
    private PlayerMovement playerMovement;
    private bool lockMovement;
    public bool start;
    private bool interact;
    public GameObject anchorUp;

    private void Start()
    {
        entered = false;
        lockMovement = false;
        start = false;
        interact = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        //Verifico se ha schiacciato spazio 10 volte per vedere se l'ancora e sollevata
        if (cont < necessaryPress)
        {
            // Verifico se e entrato un GameObject di tipo player
            if (other.tag == "Player" )
            {
                playerMovement = other.GetComponent<PlayerMovement>();
                //attivo il bottone che dice "premi spazio per salire l'ancora"
                textButton.text = "Press E to interact";
                button.gameObject.SetActive(true);
                //la variabile booleana=true indica che il giocatore e dentro il cilindro
                entered = true;
                enableOutline();
            }
            //Se non � vicino all'ancora allora non lo vede
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Se esce disattivo il bottone e la variabile entered e falsa
        if (other.tag == "Player")
        {     
            button.gameObject.SetActive(false);
            entered = false;
            playerMovement = null;

            if (anchor != null)
            {
                disableOutline();
            }
        }
        
    }

    private void Update()
    {
        if (entered == true) {

            if (interact == false)
            {
                textButton.text = "Press space to pull anchor";
            }

            if (Input.GetKeyDown("space"))
            {
                interact = true;
                //cont tiene conto del numero di volte che e stato premuto spazio
                cont++;
                textButton.text = "Space key pressed " + cont + " times";
                print("Space key pressed " + cont + " times");
                anchor.transform.position = anchor.transform.position + new Vector3(0, movementSpeed, 0);
                if (cont % 3 == 0)
                {
                    //anchor.transform.GetChild(child).gameObject.GetComponent<Outline>().enabled = false;
                    //Destroy(anchor.transform.GetChild(child).gameObject);
                    anchor.transform.GetChild(child).gameObject.SetActive(false);
                    child--;
                }
                //Se arriviamo a 120 la nave deve iniziare a muoversi e il bottone si disattiva
                if (cont == necessaryPress && start==false)
                {
                    Health_and_Speed_Manager manager = ship.GetComponent<Health_and_Speed_Manager>();
                    manager.addMaxSpeed(8f);

                    //Destroy(anchor);
                    anchor.SetActive(false);
                    anchorUp.SetActive(true);
                    button.gameObject.SetActive(false);
                    entered = false;
                    start = true;                  
                }
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
}