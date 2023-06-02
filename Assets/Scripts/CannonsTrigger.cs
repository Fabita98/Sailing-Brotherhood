using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonsTrigger : MonoBehaviour
{
    public GameObject cannonBall;
    private int cont;
    private bool lockMovement;
    private PlayerMovement playerMovement;
    public GameObject cannon1, cannon2, cannon3, cannon4, cannon5;
    public int cannonBallSpeed = 10;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        cont = 0;
        lockMovement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cont == 1) { 
            if (Input.GetKeyDown(KeyCode.E) && lockMovement == false)
            {
                player.transform.rotation = cannon1.transform.rotation * Quaternion.Euler(0, 90, 0); ;
                //Qui si ferma la visuale
                if (playerMovement != null)
                {
                    playerMovement.enabled = false; // Disabilita lo script PlayerMovement
                }
                lockMovement = true;
            }
            else if (Input.GetKeyDown(KeyCode.E) && lockMovement == true)
            {
                //Qui si sblocca la visuale e puo muoversi nuovamente
                if (playerMovement != null)
                {
                    playerMovement.enabled = true; // Disabilita lo script PlayerMovement
                }
                lockMovement = false;
            }

            if (Input.GetMouseButtonDown(0) && lockMovement == true)
            {
                //la distanza da cui deve spawnare la palla dal centro del cannone
                float spawnDistance = 2f;
                //l'altezza da cui deve partire(altrimenti parte sotto le ruote del cannone)
                float spawnHeight = 1f;
                //la forza verso l'alto per dare un moto parabolico
                float upwardForce = 5f;
                //la forza orizzontale da applicare alla palla
                float forwardForceMultiplier = 1f;

                //posizione in cui spawnare la palla
                Vector3 spawnPosition = cannon1.transform.position + cannon1.transform.right * spawnDistance +Vector3.up*spawnHeight;
                //Instanziamento della palla
                GameObject cannonBall1=Instantiate(cannonBall,spawnPosition,cannon1.transform.rotation);
                //Prendo lo script associato alla palla per cambiare la direzione
                CannonBall cannonBallScript1 = cannonBall1.GetComponent<CannonBall>();
                //Cambio la direzione
                cannonBallScript1.direction = cannon1.transform.right;
                //Prendo il rigidbody della palla
                Rigidbody rbCannonBall1 = cannonBall1.GetComponent<Rigidbody>();
                //Aggiungo una forza orizzontale
                rbCannonBall1.AddForce(cannonBallScript1.direction * cannonBallSpeed, ForceMode.VelocityChange);
                //Aggiungo una forza verticale
                rbCannonBall1.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);

                //CANNONE 2
                Vector3 spawnPosition2 = cannon2.transform.position + cannon2.transform.right * spawnDistance + Vector3.up * spawnHeight;
                GameObject cannonBall2 = Instantiate(cannonBall, spawnPosition2, cannon2.transform.rotation);
                CannonBall cannonBallScript2 = cannonBall2.GetComponent<CannonBall>();
                cannonBallScript2.direction = cannon2.transform.right;
                Rigidbody rbCannonBall2 = cannonBall2.GetComponent<Rigidbody>();
                rbCannonBall2.AddForce(cannonBallScript2.direction * cannonBallSpeed, ForceMode.VelocityChange);            
                rbCannonBall2.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);

                //CANNONE3
                Vector3 spawnPosition3 = cannon3.transform.position + cannon3.transform.right * spawnDistance + Vector3.up * spawnHeight;
                GameObject cannonBall3 = Instantiate(cannonBall, spawnPosition3, cannon3.transform.rotation);
                CannonBall cannonBallScript3 = cannonBall3.GetComponent<CannonBall>();
                cannonBallScript3.direction = cannon3.transform.right;
                Rigidbody rbCannonBall3 = cannonBall3.GetComponent<Rigidbody>();
                rbCannonBall3.AddForce(cannonBallScript3.direction * cannonBallSpeed, ForceMode.VelocityChange);
                rbCannonBall3.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);

                //CANNONE4
                Vector3 spawnPosition4 = cannon4.transform.position + cannon4.transform.right * spawnDistance + Vector3.up * spawnHeight;
                GameObject cannonBall4 = Instantiate(cannonBall, spawnPosition4, cannon4.transform.rotation);
                CannonBall cannonBallScript4 = cannonBall4.GetComponent<CannonBall>();
                cannonBallScript4.direction = cannon4.transform.right;
                Rigidbody rbCannonBall4 = cannonBall4.GetComponent<Rigidbody>();
                rbCannonBall4.AddForce(cannonBallScript4.direction * cannonBallSpeed, ForceMode.VelocityChange);
                rbCannonBall4.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);

                //CANNONE4
                Vector3 spawnPosition5 = cannon5.transform.position + cannon5.transform.right * spawnDistance + Vector3.up * spawnHeight;
                GameObject cannonBall5 = Instantiate(cannonBall, spawnPosition5, cannon4.transform.rotation);
                CannonBall cannonBallScript5 = cannonBall5.GetComponent<CannonBall>();
                cannonBallScript5.direction = cannon5.transform.right;
                Rigidbody rbCannonBall5 = cannonBall5.GetComponent<Rigidbody>();
                rbCannonBall5.AddForce(cannonBallScript5.direction * cannonBallSpeed, ForceMode.VelocityChange);
                rbCannonBall5.AddForce(Vector3.up * upwardForce, ForceMode.VelocityChange);

            }

        }

        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {   cont = 1;        
            //Qui prendiamo lo script del movimento del pirata che ha triggerato i cannoni
            playerMovement = other.GetComponent<PlayerMovement>();
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {   cont = 0;
            playerMovement = null;
            player = null;
        }
    }

}
