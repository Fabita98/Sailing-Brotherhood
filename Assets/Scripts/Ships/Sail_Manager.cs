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
    public GameObject mast;
    public AudioSource ropesound;
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
                ropesound.Play();
            }
            else if (Input.GetKeyDown("e") && sail.activeSelf == false)
            {
                sail.SetActive(!sail.activeSelf);
                hs.maxspeed += 6;
                ropesound.Play();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") { count++; entered = true;
            enableOutline();
        }
   
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player") { count--; if (count == 0)
            {
                entered = false;
                disableOutline();
            }
        }
        
    }
    public void enableOutline()
    {
        Outline outline = mast.GetComponent<Outline>();
        outline.enabled = true;
    }

    public void disableOutline()
    {
        Outline outline = mast.GetComponent<Outline>();
        outline.enabled = false;
    }
}
