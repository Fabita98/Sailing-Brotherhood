using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelTrigger : MonoBehaviour
{
    public GameObject wheel;
    public Button button;
    private int cont = 0;
    private int child = 41;
    private float movementSpeed = 0.1f;
    public GameObject ship;
    private bool entered;
    private bool modNavigazione;

    private void Start()
    {
        entered = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other name: " + other.name);
            // Verifico se e entrato un GameObject di tipo player
            if (other.tag == "Player")
            {
                //attivo il bottone che dice "premi spazio per girare il trmone"
                button.gameObject.SetActive(true);
                //la variabile booleana=true indica che il giocatore e dentro il cilindro
                entered = true;
            }
        
    }

    private void OnTriggerExit(Collider other)
    {
        //Se esce disattivo il bottone e la variabile entered e falsa
        button.gameObject.SetActive(false);
        entered = false;
    }

    private void Update()
    {
        if (entered == true)
        {
            //se schiaccia spazio e non e in modalita navigazione
            if (Input.GetKeyDown("space") && modNavigazione == false)
            {
                modNavigazione = true;
                //bisogna fermare il movimento del pirata

                //se clicca di nuovo spazio esce dalla navigazione

                //se adesso schiaccia le frecce a destra o a sinistra il timone deve ruotare e deve far ruotare la barca
                //wheel.transform.Rotate = wheel.transform.position + new Vector3(0, movementSpeed, 0);

            }
            else if (Input.GetKeyDown("space") && modNavigazione == false)
            {
                modNavigazione = false;

            }
        }
    }
}
