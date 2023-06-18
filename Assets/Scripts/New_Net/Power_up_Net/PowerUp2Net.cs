using Crest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PowerUp2Net : NetworkBehaviour
{
    [SerializeField] private Vector3 _rotation;
    private GameObject ship;
    AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
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
            GameObject barrelTrigger = shipCompleted.transform.Find("Power-upBarrelDetection").gameObject;
            BarrelTriggerNet barrel = barrelTrigger.GetComponent<BarrelTriggerNet>();
            barrel.addBarrel(3);
        }
    }

}