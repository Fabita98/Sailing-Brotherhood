using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceRewards : MonoBehaviour
{
    public GameObject WinTrack, LoseTrack;
    public GameObject Canva1, Canva2;
    bool won=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (won)
        {
            WinTrack.gameObject.SetActive(true);
            Canva1.gameObject.SetActive(true);
        }
        else 
        { 
            LoseTrack.gameObject.SetActive(true);
            Canva2.gameObject.SetActive(true);
        }
    }
}
