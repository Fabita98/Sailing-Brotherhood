using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Health_and_Speed_ManagerNet : NetworkBehaviour
{
    public float health, maxspeed, actual_speed;

    public NetworkVariable<float> syncMaxSpeed = new NetworkVariable<float>();
    public NetworkVariable<float> syncActualSpeed = new NetworkVariable<float>();
    public NetworkVariable<float> syncHealth = new NetworkVariable<float>();

    public GameObject my_ship;
    Crest.BoatProbes boatProbes;
    public Image HealthBar, SpeedBar;
    public Text SpeedValue;
    public GameObject anchorTrigger;
    private AnchorTriggerNet anchor;
    public bool lifted;
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
        anchor = anchorTrigger.GetComponent<AnchorTriggerNet>();
        if (IsServer)
        {
            if (anchor.start == true)
            {
                if (lifted == false) {
                    lifted = true;
                    maxspeed += 8; 
                }
                boatProbes = my_ship.GetComponent<Crest.BoatProbes>();

                if (health > 100)
                {
                    health = 100;
                    syncHealth.Value = health;
                }

                if (health < 0)
                {
                    health = 0;
                    syncHealth.Value = health;
                }
                if (maxspeed < 0)
                {
                    maxspeed = 0;
                    syncMaxSpeed.Value = maxspeed;
                }
                if (maxspeed > 50)
                {
                    maxspeed = 50;
                    syncMaxSpeed.Value = maxspeed;
                }
                actual_speed = maxspeed - maxspeed * (1 - health / 100);
                syncActualSpeed.Value = actual_speed;
                boatProbes._enginePower = syncActualSpeed.Value;
                
            }
            
        }
        HealthBar.fillAmount = syncHealth.Value / 100;
        SpeedBar.fillAmount = syncActualSpeed.Value / 20;
        SpeedValue.text = syncActualSpeed.Value.ToString();
    }

    public float getMaxSpeed()
    {
        return maxspeed;
    }

    public void addMaxSpeed(float f)
    {
        maxspeed += f;
        syncMaxSpeed.Value = maxspeed;
        Debug.Log("Velocità aumentata di 8");
    }
    public void decreaseMaxSpeed(float f)
    {
        maxspeed -= f;
        syncMaxSpeed.Value = maxspeed;
    }
}
