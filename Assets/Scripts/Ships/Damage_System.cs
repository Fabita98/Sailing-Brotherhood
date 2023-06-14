using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_System : MonoBehaviour
{
    public GameObject my_ship;
    Health_and_Speed_Manager hs;
    float cooldown;
    bool canbedamaged;
    public AudioSource cannonhit, terrainSound;
    // Start is called before the first frame update
    void Start()
    {
        canbedamaged = true;
        cooldown = 5;
    }
    void OnTriggerEnter(Collider other)
    {
        if (canbedamaged)
        {
            if (other.tag == "Terrain")
            {
                hs.health -= 10;
                CooldownStart();
                terrainSound.Play();

            }
            if (other.tag == "Cannonball")
            {
                hs.maxspeed -= 20;
                CooldownStart();
                cannonhit.Play();

            }
            if (other.tag == "Barrel")
            {
                hs.maxspeed -= 10;
                CooldownStart();

            }
        }



    }
    public void CooldownStart()
    {
        StartCoroutine(CooldownCoroutine());
    }
    IEnumerator CooldownCoroutine()
    {
        canbedamaged = false;
        yield return new WaitForSeconds(cooldown);
        canbedamaged = true;
    }

    // Update is called once per frame
    void Update()
    {
        hs = my_ship.GetComponent<Health_and_Speed_Manager>();
    }
}
