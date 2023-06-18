using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrotAI : MonoBehaviour
{
    bool can_speak = true;
    public bool anchor, wrong_direction, powerup_ahead, damage, cannon_hit, enemy_approaching, close_to_treasure = false;
    int count = 0;
    float cooldown = 15;
    public AudioSource AudioAnchor, AudioPowUp, AudioTreasure, AudioDirection, AudioDamage, AudioEnemy;
    public GameObject my_ship, enemy_ship;
    private Health_and_Speed_ManagerNet hs;
    private float Cosalpha, distance;
    // need to be liked to 
    void Start()
    {
        hs = my_ship.GetComponent<Health_and_Speed_ManagerNet>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (hs.health < 100) damage = true; else damage = false;
        //check if there is a >90° angle between forward of ship and direction to treasure
        Cosalpha = Vector3.Dot(my_ship.transform.forward, (new Vector3(-4514.46484f, 170, -4991.76465f).normalized- my_ship.transform.position));     
        if (Cosalpha < 0) wrong_direction = true; else wrong_direction = false;

        //check if the distance between the ship and treasure is less than 300
        distance = (new Vector3(-4514.46484f, 170, -4991.76465f) - my_ship.transform.position).magnitude;
        if (distance < 300) close_to_treasure = true; else close_to_treasure = false;
        // check if there is an enemy ship around
        if ((my_ship.transform.position - enemy_ship.transform.position).magnitude < 200) enemy_approaching = true; else enemy_approaching = false;  

        if (can_speak)
        {
            if (anchor)
            {
                CooldownStart();
                can_speak = false;
                AudioAnchor.Play();
                CooldownStart();
                cooldown = 15;
                return;
            }           
            if (close_to_treasure)
            {
                can_speak = false;
                AudioTreasure.Play();
                CooldownStart();
                return;
            }
            if (wrong_direction)
            {
                can_speak = false;
                AudioDirection.Play();
                CooldownStart();
                return;
            }
            if (damage)
            {
                can_speak = false;
                AudioDamage.Play();
                CooldownStart();
                return;
            }
            if (powerup_ahead)
            {
                can_speak = false;
                AudioPowUp.Play();
                CooldownStart();
                return;
            }
            if (enemy_approaching)
            {
                can_speak = false;
                AudioEnemy.Play();
                CooldownStart();
                return;
            }
        }

    }

    public void CooldownStart()
    {
        StartCoroutine(CooldownCoroutine());
    }
    IEnumerator CooldownCoroutine()
    {
        
        yield return new WaitForSeconds(cooldown);
        can_speak = true;
    }
}
