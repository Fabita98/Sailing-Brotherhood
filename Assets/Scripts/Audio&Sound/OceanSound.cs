using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanSound : MonoBehaviour
{
    public AudioSource slow_sound;
    public AudioSource fast_sound;
    public GameObject ship1;
    private Health_and_Speed_ManagerNet hs;
    bool isplayingfast = false;
    bool isplayingslow = true;
    // Start is called before the first frame update
    void Start()
    {
        slow_sound.Play();
        hs = ship1.GetComponent<Health_and_Speed_ManagerNet>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hs.actual_speed < 10){
            if (!isplayingslow)
            {
                slow_sound.Play();
                fast_sound.Pause();
                isplayingslow = true;
                isplayingfast = false;
            }
        } else if (!isplayingfast) {
            fast_sound.Play();
            slow_sound.Pause();
            isplayingfast = true;
            isplayingslow = false;
            }
    }
}
