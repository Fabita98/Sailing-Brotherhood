using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp3 : MonoBehaviour
{
    private GameObject ship;
    CannonsTrigger cannons1, cannons2;

    [SerializeField] private Vector3 _rotation;
    // Start is called before the first frame update
    void Start()
    {
        
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

            GameObject cannons = ship.transform.Find("Cannons").gameObject;
            GameObject rightCannons = cannons.transform.Find("On-deck_cannons_set_right").gameObject;
            GameObject leftCannons = cannons.transform.Find("On-deck_cannons_set_left").gameObject;

            GameObject rightCannonsDetection = rightCannons.transform.Find("CannonDetection_right").gameObject;
            GameObject leftCannonsDetection = leftCannons.transform.Find("CannonDetection_left").gameObject;

            cannons1=rightCannonsDetection.GetComponent<CannonsTrigger>();
            cannons2 = leftCannonsDetection.GetComponent<CannonsTrigger>();

            cannons1.holdTimeRequired = 3f;
            cannons2.holdTimeRequired = 3f;

            Invoke("oldHoldTimeRequired", 30);
        }
    }

    private void oldHoldTimeRequired()
    {
        cannons1.holdTimeRequired = 8f;
        cannons2.holdTimeRequired = 8f;
        cannons1 = null;
        cannons2 = null;

    }
}
