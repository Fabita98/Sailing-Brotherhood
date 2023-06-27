using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PowerUp3Net : NetworkBehaviour
{
    private GameObject ship;
    CannonsTriggerNet cannons1, cannons2;
    AudioSource sound;
    [SerializeField] private Vector3 _rotation;
    private PowerUpTaken powerUpTaken; // Reference to the PowerUpTaken script

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        powerUpTaken = FindObjectOfType<PowerUpTaken>(); // Find the PowerUpTaken script in the scene
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ship")
        {
            GameObject shipBody = other.transform.parent.gameObject;
            GameObject shipComponent = shipBody.transform.parent.gameObject;
            GameObject shipCompleted = shipComponent.transform.parent.gameObject;

            ship = shipCompleted.gameObject;
            sound.Play();

            this.gameObject.SetActive(false);
            GameObject cannons = ship.transform.Find("Cannons").gameObject;
            GameObject rightCannons = cannons.transform.Find("On-deck_cannons_set_right").gameObject;
            GameObject leftCannons = cannons.transform.Find("On-deck_cannons_set_left").gameObject;

            GameObject rightCannonsDetection = rightCannons.transform.Find("CannonDetection_right").gameObject;
            GameObject leftCannonsDetection = leftCannons.transform.Find("CannonDetection_left").gameObject;

            cannons1 = rightCannonsDetection.GetComponent<CannonsTriggerNet>();
            cannons2 = leftCannonsDetection.GetComponent<CannonsTriggerNet>();

            //cannons1.holdTimeRequired = 3f;
            //cannons2.holdTimeRequired = 3f;

            if (IsClient) cannons1.ReduceReloadTimeServerRPC();
            else { cannons1.ReduceReloadTimeClientRPC(); }

            if (IsClient) cannons2.ReduceReloadTimeServerRPC();
            else { cannons2.ReduceReloadTimeClientRPC(); }

            Invoke("respawnPowerUp", 20);
            Invoke("oldHoldTimeRequired", 30);
            powerUpTaken.ShowReloadPowerUpTaken(ship);
        }
    }

    private void oldHoldTimeRequired()
    {
        //cannons1.holdTimeRequired = 8f;
        //cannons2.holdTimeRequired = 8f;
        if (IsClient) cannons1.AddReloadTimeServerRPC();
        else { cannons1.AddReloadTimeClientRPC(); }

        if (IsClient) cannons2.AddReloadTimeServerRPC();
        else { cannons2.AddReloadTimeClientRPC(); }
        cannons1 = null;
        cannons2 = null;
    }

    private void respawnPowerUp()
    {
        this.gameObject.SetActive(true);
    }
}