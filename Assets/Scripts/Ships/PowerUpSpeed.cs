using Crest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : MonoBehaviour
{
    private GameObject ship;
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
            //GameObject shipComponent = other.transform.Find("Ship_Components").gameObject;
            //GameObject shipBody = other.transform.Find("Ship_Body").gameObject;
            //GameObject shipCollider = other.transform.Find("Ship_collider").gameObject;
            GameObject shipBody = other.transform.parent.gameObject;
            GameObject shipComponent = shipBody.transform.parent.gameObject;
            GameObject shipCompleted = shipComponent.transform.parent.gameObject;

            ship = shipCompleted.gameObject;
            Debug.Log("other" + other.name);
            Health_and_Speed_Manager manager = ship.GetComponent<Health_and_Speed_Manager>();
            manager.addMaxSpeed(20f);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ship")
        {
            Invoke("SlowDown", 5);
        }
    }

    public void SlowDown()
    {
        Health_and_Speed_Manager manager = ship.GetComponent<Health_and_Speed_Manager>();
        manager.decreaseMaxSpeed(20f);
    }
}