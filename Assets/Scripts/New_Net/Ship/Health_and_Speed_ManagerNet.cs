using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Mono.CSharp;

public class Health_and_Speed_ManagerNet : NetworkBehaviour
{
    public float health, maxspeed, actual_speed;

    public NetworkVariable<float> syncMaxSpeed = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> syncActualSpeed = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    //public NetworkVariable<float> syncHealth = new NetworkVariable<float>(100); 
    
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
        health = 100;
        boatProbes = my_ship.GetComponent<Crest.BoatProbes>();
        if (IsHost)
        {
            //syncHealth.Value = 100;
            syncMaxSpeed.Value = maxspeed;
            syncActualSpeed.Value = actual_speed;
        }

        if (maxspeed > 50) maxspeed = 50; // non necessario su start
        actual_speed = maxspeed - maxspeed * (1 - health / 100);
        boatProbes._enginePower = syncActualSpeed.Value /*actual_speed*/;
        HealthBar.fillAmount = 1;
        SpeedBar.fillAmount = syncActualSpeed.Value/ 20;
        SpeedValue.text = syncActualSpeed.Value.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsHost) { health = syncHealth.Value;
        //    Debug.Log("metto a 100 health prendendola da sync, health:"+health+" sync: "+syncHealth.Value);
        //}

        HealthBar.fillAmount = health / 100;
        SpeedBar.fillAmount = syncActualSpeed.Value / 20;
        SpeedValue.text = syncActualSpeed.Value.ToString();
        anchor = anchorTrigger.GetComponent<AnchorTriggerNet>();
        boatProbes = my_ship.GetComponent<Crest.BoatProbes>();
        if (IsServer)
        {
            if (anchor.start == true)
            {
                if (lifted == false) {
                    lifted = true;
                    maxspeed += 8; 
                }
                

                if (health > 100)
                {
                    health = 100;                    
                }

                if (health < 0)
                {
                    health = 0;
                }
                if (maxspeed < 0)
                {
                    maxspeed = 0;
                }
                if (maxspeed > 50)
                {
                    maxspeed = 50;
                }
              actual_speed = maxspeed - maxspeed * (1 - health / 100);
            boatProbes._enginePower = syncActualSpeed.Value;  
            }
            
   //         syncHealth.Value = health;
            syncMaxSpeed.Value = maxspeed;
            syncActualSpeed.Value = actual_speed;
            


        }
        //HealthBar.fillAmount = syncHealth.Value / 100;
        //SpeedBar.fillAmount = syncActualSpeed.Value / 20;
        //Debug.Log("Health: "+health);
        //Debug.Log("SyncHealth: " + syncHealth.Value);
        
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