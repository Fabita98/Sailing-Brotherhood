using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sailManager : MonoBehaviour
{
    public GameObject sail;
    public GameObject belongingShip;
    private Crest.BoatProbes boatProbes;
    // Start is called before the first frame update
    void Start()
    {
        boatProbes = belongingShip.GetComponent<Crest.BoatProbes>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown("e") && sail.activeSelf) { 
                sail.SetActive(!sail.activeSelf);
                boatProbes._enginePower -= 2; 
            } else if (Input.GetKeyDown("e") && sail.activeSelf==false)
            {
                sail.SetActive(!sail.activeSelf);
                boatProbes._enginePower += 2;
            }
        }
    }
}
