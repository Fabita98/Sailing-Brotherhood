using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_and_Speed_Manager : MonoBehaviour
{
    public float health, maxspeed, actual_speed;
    public GameObject my_ship;
    Crest.BoatProbes boatProbes;
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        maxspeed = 0;
        actual_speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        boatProbes = my_ship.GetComponent<Crest.BoatProbes>();
        if (health > 100) health = 100;
        if (health <0) health = 0;
        if (maxspeed < 0) maxspeed = 0;
        if (maxspeed > 50) maxspeed = 50;
        actual_speed = maxspeed - maxspeed * (1-health/100);
        boatProbes._enginePower = actual_speed;
    }
}
