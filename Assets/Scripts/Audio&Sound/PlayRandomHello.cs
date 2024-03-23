using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomHello : MonoBehaviour
{
    public static PlayRandomHello playRnd;
    private AudioSource sound;
    private AudioClip[] HelloList;
    private int rnd;
    // Start is called before the first frame update
    void Start()
    {
        playRnd = this;
        sound = GetComponent<AudioSource>();
        HelloList = Resources.LoadAll<AudioClip>("Mysterious_man/hello");
    }

    // Update is called once per frame
    public void PlayButtonHello()
    {
        rnd = Random.Range(0, HelloList.Length);
        sound.PlayOneShot(HelloList[rnd]);


    }
}
