using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonsTrigger : MonoBehaviour
{
    public GameObject cannonBall;
    private int cont;
    private bool lockMovement;
    private PlayerMovement playerMovement;
    public GameObject cannon1, cannon2, cannon3, cannon4, cannon5;
    public int cannonBallSpeed = 10;
    private bool shooted;

    public Button button;
    public Text textButton;

    private bool entered;
    public float holdTimeRequired = 10f;
    private float holdTime = 0f;

    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        cont = 0;
        lockMovement = false;
        shooted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (entered==true) { 
            if (Input.GetKeyDown(KeyCode.E) && lockMovement == false)
            {
                player.transform.rotation = cannon1.transform.rotation * Quaternion.Euler(0, 90, 0);
                
                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = true;

                //Qui si ferma la visuale
                if (playerMovement != null)
                {
                    //player.GetComponent<Rigidbody>().constraints= RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; ;
                    playerMovement.speed = 0;
                }
                lockMovement = true;

                if (shooted == true)
                {
                    textButton.text = "Click R to reload";
                }
                else
                {
                    //Il bottone mostrer� il testo Left click to shoot per sparare
                    textButton.text = "Left click to shoot";
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && lockMovement == true)
            {
                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = false;
                //Qui si sblocca la visuale e puo muoversi nuovamente
                if (playerMovement != null)
                {
                    playerMovement.speed = 10;
                }
                lockMovement = false;

                if (shooted == true)
                {
                    textButton.text = "Click R to reload";
                }
                else { 
                //Il bottone mostrer� il testo Click E to interact per sparare
                textButton.text = "Click E to interact";
                }
            }

            if (Input.GetMouseButtonDown(0) && lockMovement == true&& shooted==false)
            {
               
                //la forza orizzontale da applicare alla palla
                Camera cameraPlayer = player.GetComponentInChildren<Camera>();
                Vector3 upwardDirection = cameraPlayer.transform.forward;

                shooting(cannon1,upwardDirection);
                shooting(cannon2, upwardDirection);
                shooting(cannon3, upwardDirection);
                shooting(cannon4, upwardDirection);
                shooting(cannon5, upwardDirection);

                shooted = true;

                //Il bottone mostrer� il testo Click R to reload
                textButton.text = "Click R to reload";
            }

            if (Input.GetKey(KeyCode.R) && shooted == true)
            {
                holdTime += Time.deltaTime;
                Debug.Log("Holdtime: " + holdTime);
                float waitingTime = 10 - holdTime;
                string formattedValue = waitingTime.ToString("F2");
                textButton.text = ""+formattedValue;
                if (holdTime >= holdTimeRequired)
                {
                    shooted = false;
                    if(lockMovement == true)
                    {
                        //Il bottone mostrer� il testo Left click to shoot per sparare
                        textButton.text = "Left click to shoot";
                    }
                    else
                    {
                        //Il bottone mostrer� il testo Left click to shoot per sparare
                        textButton.text = "Click E to interact";
                    }
                }           
            }
            else
            {
                holdTime = 0f;
                //Il bottone mostrer� il testo Click R to reload
                if (shooted == true) {
                    textButton.text = "Click R to reload";
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {   //la variabile booleana=true indica che il giocatore e dentro il cilindro
            entered = true;
            //Qui prendiamo lo script del movimento del pirata che ha triggerato i cannoni
            playerMovement = other.GetComponent<PlayerMovement>();
            player = other.gameObject;
            //attivo il bottone che dice "premi E per interagire"
            button.gameObject.SetActive(true);         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {   cont = 0;
            playerMovement = null;
            player = null;
            //Se esce disattivo il bottone e la variabile entered e falsa
            button.gameObject.SetActive(false);
            entered = false;
        }
    }

    private void shooting(GameObject cannon, Vector3 upwardDirection)
    {
        //la distanza da cui deve spawnare la palla dal centro del cannone
        float spawnDistance = 2f;
        //l'altezza da cui deve partire(altrimenti parte sotto le ruote del cannone)
        float spawnHeight = 1f;
        //la forza verso l'alto per dare un moto parabolico
        float upwardForce = 8f;
        //la forza orizzontale da applicare alla palla
        float forwardForceMultiplier = 1f;
        //Dove guarda il giocatore
        //Vector3 upwardDirection = cameraPlayer.transform.up;

        //posizione in cui spawnare la palla
        Vector3 spawnPosition = cannon.transform.position + cannon.transform.right * spawnDistance + Vector3.up * spawnHeight;
        //Instanziamento della palla
        GameObject cannonBall1 = Instantiate(cannonBall, spawnPosition, cannon.transform.rotation);
        //Prendo lo script associato alla palla per cambiare la direzione
        CannonBall cannonBallScript1 = cannonBall1.GetComponent<CannonBall>();
        //Cambio la direzione
        cannonBallScript1.direction = cannon.transform.right;
        //Prendo il rigidbody della palla
        Rigidbody rbCannonBall1 = cannonBall1.GetComponent<Rigidbody>();
        //Aggiungo una forza orizzontale
        rbCannonBall1.AddForce(cannonBallScript1.direction * cannonBallSpeed, ForceMode.VelocityChange);
        //Aggiungo una forza verticale
        //SE VUOI MODIFICARE QUANTO DISTANTI VANNO LE PALLE DI CANNONE MOLTIPICA PER UN VALORE esempio: upwardDirection*upwardForce*5
        rbCannonBall1.AddForce(upwardDirection * upwardForce*3, ForceMode.VelocityChange);
    }
}