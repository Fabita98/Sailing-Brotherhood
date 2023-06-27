using Crest;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpSpeedNet : NetworkBehaviour
{
    public GameObject ship;
    [SerializeField] private Vector3 _rotation;
    AudioSource sound;

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

    public void OnTriggerEnter(Collider other)

    {
        if (other.tag == "Ship")
        {
            //GameObject shipComponent = other.transform.Find("Ship_Components").gameObject;
            //GameObject shipBody = other.transform.Find("Ship_Body").gameObject;
            //GameObject shipCollider = other.transform.Find("Ship_collider").gameObject;
            GameObject shipBody = other.transform.parent.gameObject;
            GameObject shipComponent = shipBody.transform.parent.gameObject;
            GameObject shipCompleted = shipComponent.transform.parent.gameObject;
            sound.Play();
            this.gameObject.SetActive(false);
            ship = shipCompleted.gameObject;
            Debug.Log("other" + other.name);
            Health_and_Speed_ManagerNet manager = ship.GetComponent<Health_and_Speed_ManagerNet>();
            manager.addMaxSpeed(20f);

            Invoke("SlowDown", 10);
            Invoke("respawnPowerUp", 20);
            // Show the power-up taken UI
            powerUpTaken.ShowSpeedPowerUpTaken(ship);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /*if (other.tag == "Ship")
        {
            Invoke("SlowDown", 10);
        }*/
    }

    public void SlowDown()
    {
        Health_and_Speed_ManagerNet manager = ship.GetComponent<Health_and_Speed_ManagerNet>();
        manager.decreaseMaxSpeed(20f);
    }

    private void respawnPowerUp()
    {
        this.gameObject.SetActive(true);
    }
}
