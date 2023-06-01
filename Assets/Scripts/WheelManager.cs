using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject wheel;
    public GameObject belongingShip;
    private Crest.BoatProbes boatProbes;
    bool active = false;
    public GameObject driver;
    private PlayerController PirateMovement;
    // Start is called before the first frame update
    void Start()
    {
        boatProbes = belongingShip.GetComponent<Crest.BoatProbes>();
        PirateMovement = driver.GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            if (Input.GetKeyDown("e"))
            {
                active = !active;              
            }
            if (active)
            {
                PirateMovement.driving = true;
                float x = Input.GetAxis("Horizontal");
                boatProbes._turnBias += x * 0.005f;
                wheel.gameObject.transform.Rotate(0, 0, x);
            }
            else PirateMovement.driving = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PirateMovement.driving = false;
    }
    }
