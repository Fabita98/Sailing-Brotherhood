using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class CannonsTriggerNet : NetworkBehaviour
{
    public GameObject cannonBall;
    private int cont;
    private bool lockMovement;
    private PlayerMovementNet playerMovement;
    public GameObject cannon1, cannon2, cannon3, cannon4, cannon5, cannon6;
    public GameObject effectCannon1, effectCannon2, effectCannon3, effectCannon4, effectCannon5, effectCannon6;

    public int cannonBallSpeed = 10;
    private bool shooted;

    public Button button;
    public Text textButton;

    private bool entered;
    public float holdTimeRequired = 8f;
    private float holdTime = 0f;

    public AudioSource cannonsound, reloadSound;
    bool isplaying = false;
    bool cannonOccupied;

    private GameObject player;
    // Start is called before the first frame update

    //la distanza da cui deve spawnare la palla dal centro del cannone
    float spawnDistance = 2f;
    //l'altezza da cui deve partire(altrimenti parte sotto le ruote del cannone)
    float spawnHeight = 1f;
    //la forza verso l'alto per dare un moto parabolico
    float upwardForce = 24f;
    //la forza orizzontale da applicare alla palla
    float forwardForceMultiplier = 1f;
    //Dove guarda il giocatore

    public GameObject arrow1, arrow2, arrow3, arrow4, arrow5, arrow6;

    Vector3 upwardDirection;

    void Start()
    {
        cont = 0;
        lockMovement = false;
        shooted = false;
        cannonOccupied = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (entered == true)
        {

            if (lockMovement == true && shooted == true)
            {
                textButton.text = "Click R to reload";
            }

            if (lockMovement == true && shooted == false)
            {
                textButton.text = "Left click to shoot";
            }

            if (lockMovement == false && shooted == true)
            {
                textButton.text = "Click R to reload";
            }

            if (lockMovement == false && shooted == false)
            {
                textButton.text = "Press E to interact";
            }

            if (lockMovement == true)
            {
                Camera cameraPlayer = player.GetComponentInChildren<Camera>();
                //Vector3 upwardDirection = cameraPlayer.transform.forward;

                trajectoryShooting(cameraPlayer.transform);
            }
            Debug.Log("Il cannone occupato:" + cannonOccupied);

            if (cannonOccupied == false && lockMovement == false && Input.GetKeyDown(KeyCode.E))
            {
                cannonOccupied = true;
                Debug.Log("Hai schiacciato E e cannonOccupied:" + cannonOccupied);

                if (IsClient) CannonOccupiedServerRPC();
                else { CannonOccupiedClientRPC(); }

                player.transform.rotation = cannon1.transform.rotation * Quaternion.Euler(0, 90, 0);

                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = true;

                //Qui si ferma la visuale
                if (playerMovement != null)
                {
                    // Blocca il movimento del giocatore chiamando il metodo LockMovement
                    playerMovement.LockMovement();
                }
                lockMovement = true;

                if (shooted == true)
                {
                    textButton.text = "Press R to reload";
                }
                else
                {
                    //Il bottone mostrerà il testo Left click to shoot per sparare
                    textButton.text = "Left click to shoot";
                }
            }
            else if (lockMovement == true && Input.GetKeyDown(KeyCode.E))
            {
                cannonOccupied = false;
                if (IsClient) CannonNotOccupiedServerRPC();
                else { CannonNotOccupiedClientRPC(); }

                player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = false;
                //Qui si sblocca la visuale e puo muoversi nuovamente
                if (playerMovement != null)
                {
                    playerMovement.UnlockMovement();
                }
                lockMovement = false;
                disableTrajectoryShooting();

                if (shooted == true)
                {
                    textButton.text = "Press R to reload";
                }
                else
                {
                    //Il bottone mostrerà il testo Click E to interact per sparare
                    textButton.text = "Press E to interact";
                }
            }

            if (lockMovement == true && shooted == false && Input.GetMouseButtonDown(0))
            {
                //la forza orizzontale da applicare alla palla
                Camera cameraPlayer = player.GetComponentInChildren<Camera>();
                upwardDirection = cameraPlayer.transform.forward;

                /*shooting(cannon1, upwardDirection);
                shooting(cannon2, upwardDirection);
                shooting(cannon3, upwardDirection);
                shooting(cannon4, upwardDirection);
                shooting(cannon5, upwardDirection);
                shooting(cannon6, upwardDirection);*/

                if (IsClient) CannonBallServerRPC();
                else { CannonBallClientRPC(); }

                /*cannonsound.Play();

                effectCannon1.SetActive(true);
                effectCannon2.SetActive(true);
                effectCannon3.SetActive(true);
                effectCannon4.SetActive(true);
                effectCannon5.SetActive(true);
                effectCannon6.SetActive(true);*/

                shooted = true;
                if (IsClient) ShootedServerRPC();
                else { ShootedClientRPC(); }

                //Il bottone mostrerà il testo Click R to reload
                textButton.text = "Press R to reload";

                Invoke("disableEffects", 3);
            }

            if (Input.GetKey(KeyCode.R) && shooted == true)
            {
                if (!isplaying)
                {
                    reloadSound.Play();
                    isplaying = true;
                }
                holdTime += Time.deltaTime;
                Debug.Log("Holdtime: " + holdTime);
                float waitingTime = holdTimeRequired - holdTime;
                string formattedValue = waitingTime.ToString("F2");
                textButton.text = "" + formattedValue;
                if (holdTime >= holdTimeRequired)
                {
                    isplaying = false;
                    reloadSound.Stop();
                    shooted = false;

                    if (IsClient) NotShootedServerRPC();
                    else { NotShootedClientRPC(); }

                    if (lockMovement == true)
                    {
                        //Il bottone mostrerà il testo Left click to shoot per sparare
                        textButton.text = "Left click to shoot";
                    }
                    else
                    {
                        //Il bottone mostrerà il testo Left click to shoot per sparare
                        textButton.text = "Press E to interact";
                    }
                }
            }
            else
            {
                holdTime = 0f;
                isplaying = false;
                reloadSound.Stop();
                //Il bottone mostrerà il testo Click R to reload
                if (shooted == true)
                {
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
            if (playerMovement == null)
            {
                playerMovement = other.GetComponent<PlayerMovementNet>();
            }
            if (player == null) { player = other.gameObject; }
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
        {
            //player = other.gameObject;
            //playerMovement = other.GetComponent<PlayerMovementNet>();
            if (player != null)
            {
                if (player.GetComponentInChildren<FirstPersonCamera>() != null)
                    player.GetComponentInChildren<FirstPersonCamera>().lockHorizontalRotation = false;
                //Qui si sblocca la visuale e puo muoversi nuovamente
                if (playerMovement != null)
                {
                    playerMovement.UnlockMovement();
                }
                lockMovement = false;
                disableTrajectoryShooting();

                cont = 0;
                playerMovement = null;
                player = null;
                cannonOccupied = false;
            }
            if (IsClient) CannonNotOccupiedServerRPC();
            else { CannonNotOccupiedClientRPC(); }

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
        rbCannonBall1.AddForce(upwardDirection * upwardForce, ForceMode.VelocityChange);

        Destroy(cannonBall1, 10);
    }

    private void trajectoryShooting(Transform camera)
    {
        arrow1.SetActive(true);
        arrow2.SetActive(true);
        arrow3.SetActive(true);
        arrow4.SetActive(true);
        arrow5.SetActive(true);
        arrow6.SetActive(true);

        arrow1.transform.rotation = camera.rotation;
        arrow2.transform.rotation = camera.rotation;
        arrow3.transform.rotation = camera.rotation;
        arrow4.transform.rotation = camera.rotation;
        arrow5.transform.rotation = camera.rotation;
        arrow6.transform.rotation = camera.rotation;
    }

    private void disableTrajectoryShooting()
    {
        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow3.SetActive(false);
        arrow4.SetActive(false);
        arrow5.SetActive(false);
        arrow6.SetActive(false);
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

    [ServerRpc(RequireOwnership = false)]
    private void ShootedServerRPC()
    {
        ShootedClientRPC();
    }

    [ClientRpc]
    private void ShootedClientRPC()
    {
        shooted = true;
        textButton.text = "Click R to reload";
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotShootedServerRPC()
    {
        NotShootedClientRPC();
    }

    [ClientRpc]
    private void NotShootedClientRPC()
    {
        shooted = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CannonBallServerRPC()
    {
        CannonBallClientRPC();
    }

    [ClientRpc]
    private void CannonBallClientRPC()
    {
        shooted = true;
        shooting(cannon1, upwardDirection);
        shooting(cannon2, upwardDirection);
        shooting(cannon3, upwardDirection);
        shooting(cannon4, upwardDirection);
        shooting(cannon5, upwardDirection);
        shooting(cannon6, upwardDirection);
        cannonsound.Play();
        effectCannon1.SetActive(true);
        effectCannon2.SetActive(true);
        effectCannon3.SetActive(true);
        effectCannon4.SetActive(true);
        effectCannon5.SetActive(true);
        effectCannon6.SetActive(true);

        Invoke("disableEffects", 3);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CannonOccupiedServerRPC()
    {
        CannonOccupiedClientRPC();
    }

    [ClientRpc]
    private void CannonOccupiedClientRPC()
    {
        cannonOccupied = true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void CannonNotOccupiedServerRPC()
    {
        CannonNotOccupiedClientRPC();
    }

    [ClientRpc]
    private void CannonNotOccupiedClientRPC()
    {
        cannonOccupied = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReduceReloadTimeServerRPC()
    {
        ReduceReloadTimeClientRPC();
    }

    [ClientRpc]
    public void ReduceReloadTimeClientRPC()
    {
        holdTimeRequired = 3;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddReloadTimeServerRPC()
    {
        AddReloadTimeClientRPC();
    }

    [ClientRpc]
    public void AddReloadTimeClientRPC()
    {
        holdTimeRequired = 8;
    }

}