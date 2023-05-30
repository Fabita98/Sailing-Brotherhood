using Crest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : MonoBehaviour
{
    GameObject ship;
    [SerializeField] private Vector3 _rotation;
    BoatProbes boatProbes;
    public float originalEnginePower;
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
                boatProbes = other.GetComponent<BoatProbes>();
                originalEnginePower=boatProbes.getEnginePower();
                boatProbes.setEnginePower(40f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        boatProbes = other.GetComponent<BoatProbes>();
        //yield return new WaitForSeconds(5);
        //boatProbes.setEnginePower(originalEnginePower);
       Invoke("SlowDown",5);
    }

    public void SlowDown()
    {
        boatProbes.setEnginePower(originalEnginePower); ;
    }
}
