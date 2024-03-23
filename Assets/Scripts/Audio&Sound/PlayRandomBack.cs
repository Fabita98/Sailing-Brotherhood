using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomBack : MonoBehaviour
{
    public static PlayRandomBack playRnd;
    private AudioSource sound;
    private AudioClip[] backList;
    private int rnd;
    // Start is called before the first frame update
    void Start()
    {
        playRnd = this;
        sound = GetComponent<AudioSource>();
        backList = Resources.LoadAll<AudioClip>("Mysterious_man/back");
    }

    // Update is called once per frame
    public void PlayButtonBack()
    {
        rnd = Random.Range(0, backList.Length);
        sound.PlayOneShot(backList[rnd]);


    }
}
