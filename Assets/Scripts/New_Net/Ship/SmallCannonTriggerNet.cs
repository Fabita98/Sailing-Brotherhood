using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class SmallCannonTriggerNet : NetworkBehaviour
{
    public GameObject smallCannon;
    public GameObject cannonBall;
    public int cannonBallSpeed = 10;
    private bool entered;
    private GameObject player;
    public Button button;
    public Text textButton;
    public GameObject effectCannon;
    public int cont;
    private GameObject ball;
    public AudioSource cannonSound;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (entered == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && cont == 0)
            {
                textButton.text = "Number of cannonBalls: " + cont + "\n" + "Press E to shoot";
            }

            else if (Input.GetKeyDown(KeyCode.E) && cont > 0)
            {
                //qua lancia palla di cannone               
                Debug.Log("Sei entrato");
                //la distanza da cui deve spawnare la palla dal centro del cannone
                if (IsClient) GoldenCannonBallServerRPC();
                else { GoldenCannonBallClientRPC(); }
                //GoldenShoot();
                //Qua bisogna prendere il riferimento alla Healt dell'altra barca e togliere il danno.
            }
        }
    }

    private void GoldenShoot()
    {
        cont--;
        textButton.text = "Number of cannonBalls: " + cont + "\n" + "Press E to shoot";
        float spawnDistance = 2f;
        //l'altezza da cui deve partire(altrimenti parte sotto le ruote del cannone)
        float spawnHeight = 5f;
        //Vector3 upwardDirection = cameraPlayer.transform.up;
        cannonSound.Play();
        //posizione in cui spawnare la palla
        Vector3 spawnPosition = smallCannon.transform.position + smallCannon.transform.up * spawnDistance + Vector3.up * spawnHeight;
        //Instanziamento della palla
        GameObject cannonBall1 = Instantiate(cannonBall, spawnPosition, smallCannon.transform.rotation);
        cannonBall1.tag = "GoldenCannonBall";
        //Prendo lo script associato alla palla per cambiare la direzione
        CannonBall cannonBallScript1 = cannonBall1.GetComponent<CannonBall>();
        //Cambio la direzione
        cannonBallScript1.direction = smallCannon.transform.up;
        //Prendo il rigidbody della palla
        Rigidbody rbCannonBall1 = cannonBall1.GetComponent<Rigidbody>();
        //Aggiungo una forza verticale
        rbCannonBall1.AddForce(cannonBallScript1.direction * cannonBallSpeed, ForceMode.VelocityChange);
        effectCannon.SetActive(true);
        ball = cannonBall1;
        Invoke("disableEffects", 3);
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
        Outline outline = smallCannon.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = smallCannon.GetComponent<Outline>();
        outline.enabled = false;
    }

    public void addCannonBalls(int number)
    {
        cont += number;
        textButton.text = "Number of cannonBalls: " + cont + "\n" + "Press E to shoot";
    }

    public void disableEffects()
    {
        if (ball != null)
            Destroy(ball);
        effectCannon.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddCannonBallsGoldServerRPC()
    {
        AddCannonBallsGoldClientRPC();
    }

    [ClientRpc]
    public void AddCannonBallsGoldClientRPC()
    {
        addCannonBalls(1);
    }

    [ServerRpc(RequireOwnership = false)]
    public void GoldenCannonBallServerRPC()
    {
        GoldenCannonBallClientRPC();
    }

    [ClientRpc]
    public void GoldenCannonBallClientRPC()
    {
        GoldenShoot();
    }
}