using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PowerUp4Net : NetworkBehaviour
{
    [SerializeField] private Vector3 _rotation;
    private GameObject ship;
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
            GameObject smallCannon = shipCompleted.transform.Find("SmallCannonDetection").gameObject;
            SmallCannonTriggerNet smallCannonTrigger = smallCannon.GetComponent<SmallCannonTriggerNet>();
            smallCannonTrigger.addCannonBalls(1);

            Invoke("respawnPowerUp", 20);
            powerUpTaken.ShowSmallcannonPowerUpTaken(ship);
        }
    }

    private void respawnPowerUp()
    {
        this.gameObject.SetActive(true);
    }
}