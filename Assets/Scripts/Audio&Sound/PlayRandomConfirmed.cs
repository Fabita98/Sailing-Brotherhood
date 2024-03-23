using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomConfirmed : MonoBehaviour
{
    public static PlayRandomConfirmed playRnd;
    private AudioSource sound;
    private AudioClip[] ConfirmList;
    private int rnd;
    // Start is called before the first frame update
    void Start()
    {
        playRnd = this;
        sound = GetComponent<AudioSource>();
        ConfirmList = Resources.LoadAll<AudioClip>("Mysterious_man/confirm");
    }

    // Update is called once per frame
    public void PlayButtonConfirm()
    {
        rnd = Random.Range(0, ConfirmList.Length);
        sound.PlayOneShot(ConfirmList[rnd]);


    }
}
