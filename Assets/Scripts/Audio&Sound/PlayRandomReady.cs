using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomReady : MonoBehaviour
{
    public static PlayRandomReady playRnd;
    private AudioSource sound;
    private AudioClip[] readyList;
    private int rnd;
    // Start is called before the first frame update
    void Start()
    {
        playRnd = this;
        sound = GetComponent<AudioSource>();
        readyList = Resources.LoadAll<AudioClip>("Sailor");
    }

    // Update is called once per frame
    public void PlayButtonReady()
    {
        rnd = Random.Range(0, readyList.Length);
        sound.PlayOneShot(readyList[rnd]);

    }
}
