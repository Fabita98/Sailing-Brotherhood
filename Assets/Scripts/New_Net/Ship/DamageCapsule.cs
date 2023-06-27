using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCapsule : MonoBehaviour
{

    /*public GameObject my_ship;
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
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("ti sei schiantato");
        if (canbedamaged)
        {
            if (other.gameObject.tag == "Terrain")
            {
                hs.health -= 20;
                CooldownStart();
                terrainSound.Play();
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
    }*/
}
