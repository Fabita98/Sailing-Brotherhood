using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTrigger : MonoBehaviour
{
    public GameObject cannonBall;
    private int cont;
    // Start is called before the first frame update
    void Start()
    {
        cont = 0;
    }

    private void Update()
    {
         
            //attivo il bottone che dice "premi spazio per salire l'ancora"
       if (Input.GetKeyDown(KeyCode.Space))
       {
            if (cont == 1)
            {
                GameObject buf = Instantiate(cannonBall);
                buf.transform.position = this.transform.position;
                buf.GetComponent<CannonBall>().setDirection(transform.right);
                buf.transform.rotation = transform.rotation;
            }
            //la variabile booleana=true indica che il giocatore e dentro il cilindro
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
          { cont = 1; }
 
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        { cont = 0; }
    }
}
