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
    public GameObject cannon1, cannon2, cannon3, cannon4, cannon5, cannon6;
    public GameObject effectCannon1, effectCannon2, effectCannon3, effectCannon4, effectCannon5, effectCannon6;

    public int cannonBallSpeed = 10;
    private bool shooted;

    public Button button;
    public Text textButton;

    private bool entered;
    public float holdTimeRequired = 8f;
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
                    //playerMovement.speed = 0;
                    //PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

                    // Blocca il movimento del giocatore chiamando il metodo LockMovement
                    playerMovement.LockMovement();
                    //playerMovement.lockMovement = true;

                }
                lockMovement = true;

                if (shooted == true)
                {
                    textButton.text = "Click R to reload";
                }
                else
                {
                    //Il bottone mostrerà il testo Left click to shoot per sparare
                    textButton.text = "Left click to shoot";
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && lockMovement == true)
            {
                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = false;
                //Qui si sblocca la visuale e puo muoversi nuovamente
                if (playerMovement != null)
                {
                    playerMovement.UnlockMovement();
                }
                lockMovement = false;

                if (shooted == true)
                {
                    textButton.text = "Click R to reload";
                }
                else { 
                //Il bottone mostrerà il testo Click E to interact per sparare
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
                shooting(cannon6, upwardDirection);

                effectCannon1.SetActive(true);
                effectCannon2.SetActive(true);
                effectCannon3.SetActive(true);
                effectCannon4.SetActive(true);
                effectCannon5.SetActive(true);
                effectCannon6.SetActive(true);

                shooted = true;

                //Il bottone mostrerà il testo Click R to reload
                textButton.text = "Click R to reload";

                Invoke("disableEffects", 3);
            }

            if (Input.GetKey(KeyCode.R) && shooted == true)
            {
                holdTime += Time.deltaTime;
                Debug.Log("Holdtime: " + holdTime);
                float waitingTime = holdTimeRequired - holdTime;
                string formattedValue = waitingTime.ToString("F2");
                textButton.text = ""+formattedValue;
                if (holdTime >= holdTimeRequired)
                {
                    shooted = false;
                    if(lockMovement == true)
                    {
                        //Il bottone mostrerà il testo Left click to shoot per sparare
                        textButton.text = "Left click to shoot";
                    }
                    else
                    {
                        //Il bottone mostrerà il testo Left click to shoot per sparare
                        textButton.text = "Click E to interact";
                    }
                }           
            }
            else
            {
                holdTime = 0f;
                //Il bottone mostrerà il testo Click R to reload
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

            enableOutline(cannon1);
            enableOutline(cannon2);
            enableOutline(cannon3);
            enableOutline(cannon4);
            enableOutline(cannon5);
            enableOutline(cannon6);
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

            disableOutline(cannon1);
            disableOutline(cannon2);
            disableOutline(cannon3);
            disableOutline(cannon4);
            disableOutline(cannon5);
            disableOutline(cannon6);
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
        cannonBall1.tag = "CannonBall";
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
    public void enableOutline(GameObject cannon)
    {
        Outline outline = cannon.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline(GameObject cannon)
    {
        Outline outline = cannon.GetComponent<Outline>();
        outline.enabled = false;
    }

    public void disableEffects()
    {
        effectCannon1.SetActive(false);
        effectCannon2.SetActive(false);
        effectCannon3.SetActive(false);
        effectCannon4.SetActive(false);
        effectCannon5.SetActive(false);
        effectCannon6.SetActive(false);
    }
}