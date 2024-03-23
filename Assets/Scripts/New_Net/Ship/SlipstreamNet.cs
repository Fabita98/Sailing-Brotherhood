using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SlipstreamNet : NetworkBehaviour
{
    bool is_giving;
    public string ship_name;
    public GameObject enemy_ship;
    Health_and_Speed_ManagerNet hs;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ship")
        {
            is_giving = true;
            ship_name = other.name;
            hs.maxspeed += 10;
         
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Ship")
        {
            is_giving = false;
            ship_name = "";
            hs.maxspeed -= 10;
        }
    }
        // Update is called once per frame
        void Update()
    {
        hs = enemy_ship.GetComponent<Health_and_Speed_ManagerNet>();
    }
}
