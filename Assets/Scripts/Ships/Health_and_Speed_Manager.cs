using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Health_and_Speed_Manager : MonoBehaviour
{
    public float health, maxspeed, actual_speed;
    public GameObject my_ship;
    Crest.BoatProbes boatProbes;
    public Image HealthBar, SpeedBar;
    public Text SpeedValue;
    public GameObject anchorTrigger;
    private AnchorTrigger anchor;
    // Start is called before the first frame update
    void Start()
    {
        boatProbes = my_ship.GetComponent<Crest.BoatProbes>();
        health = 100;
        maxspeed = 0;
        actual_speed = 0;
        if (maxspeed > 50) maxspeed = 50;
        actual_speed = maxspeed - maxspeed * (1 - health / 100);
        boatProbes._enginePower = actual_speed;
        HealthBar.fillAmount = health / 100;
        SpeedBar.fillAmount = actual_speed / 20;
        SpeedValue.text = actual_speed.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        anchor = anchorTrigger.GetComponent<AnchorTrigger>();
        if (anchor.start == true)
        {
            boatProbes = my_ship.GetComponent<Crest.BoatProbes>();
            if (health > 100) health = 100;
            if (health < 0) health = 0;
            if (maxspeed < 0) maxspeed = 0;
            if (maxspeed > 50) maxspeed = 50;
            actual_speed = maxspeed - maxspeed * (1 - health / 100);
            boatProbes._enginePower = actual_speed;
            HealthBar.fillAmount = health / 100;
            SpeedBar.fillAmount = actual_speed / 20;

            SpeedValue.text = actual_speed.ToString();
        }
    }

    public float getMaxSpeed()
    {
        return maxspeed;
    }

    public void addMaxSpeed(float f)
    {
        maxspeed += f;
    }
    public void decreaseMaxSpeed(float f)
    {
        maxspeed -= f;
    }
}
