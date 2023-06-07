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
    private int cont = 0;
    private int child = 41;
    private float movementSpeed = 0.1f;
    public GameObject ship;
    private bool entered;

    private void Start()
    {
        entered = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other name: " + other.name);
        //Verifico se ha schiacciato spazio 120 volte per vedere se l'ancora e sollevata
        if (cont < 120)
        {
            // Verifico se e entrato un GameObject di tipo player
            if (other.tag == "Player" )
            {
                //attivo il bottone che dice "premi spazio per salire l'ancora"
                //button.gameObject.SetActive(true);
                //la variabile booleana=true indica che il giocatore e dentro il cilindro
                entered = true;
            }
            //Se non è vicino all'ancora allora non lo vede
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Se esce disattivo il bottone e la variabile entered e falsa
        //button.gameObject.SetActive(false);
        entered = false;
    }

    private void Update()
    {
        if (entered == true) {
            if (Input.GetKeyDown("space"))
            {
                //cont tiene conto del numero di volte che e stato premuto spazio
                cont++;
                print("space key pressed " + cont + " times");
                anchor.transform.position = anchor.transform.position + new Vector3(0, movementSpeed, 0);
                if (cont % 3 == 0)
                {
                    Destroy(anchor.transform.GetChild(child).gameObject);
                    child--;
                }
                //Se arriviamo a 120 la nave deve iniziare a muoversi e il bottone si disattiva
                if (cont == 120)
                {
                    button.gameObject.SetActive(false);
                    entered = false;
                }
            }
        }
    }
}
