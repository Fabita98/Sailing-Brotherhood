using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sail_Manager : MonoBehaviour
{
    public GameObject sail;
    public GameObject belongingShip;
    Health_and_Speed_Manager hs;
    bool entered = false;
    int count = 0;
    // need to be liked to 
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        hs = belongingShip.GetComponent<Health_and_Speed_Manager>();
        if (entered)
        {
            if (Input.GetKeyDown("e") && sail.activeSelf)
            {
                sail.SetActive(!sail.activeSelf);
                hs.maxspeed -= 6;
            }
            else if (Input.GetKeyDown("e") && sail.activeSelf == false)
            {
                sail.SetActive(!sail.activeSelf);
                hs.maxspeed += 6;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { count++; entered = true; }
   
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player") { count--; if (count == 0)
            {
                entered = false;
            }
        }

    }
}
