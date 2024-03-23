using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Damage_SystemNet : NetworkBehaviour
{
    public GameObject my_ship;
    Health_and_Speed_ManagerNet hs;
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
                hs.health -= 20;
                CooldownStart();
                terrainSound.Play();
            }
            if (other.tag == "CannonBall")
            {
                hs.health -= 20;
                CooldownStart();
                cannonhit.Play();
            }
            if (other.gameObject.tag == "Barrel")
            {
                Destroy(other.gameObject);
                hs.health -= 15;
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
        hs = my_ship.GetComponent<Health_and_Speed_ManagerNet>();
    }
}
