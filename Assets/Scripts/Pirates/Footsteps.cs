using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    float playerHorizontalInput;
    float playerVerticalInput;
    bool is_walking, is_playing = false;
    public AudioSource footsteps;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerHorizontalInput = Input.GetAxis("Horizontal");
        playerVerticalInput = Input.GetAxis("Vertical");
        if (playerHorizontalInput != 0 || playerVerticalInput != 0)
        {
            is_walking = true;
        }
        else is_walking = false;

        if (!is_walking)
        {
            footsteps.Pause();
            is_playing = false;
        }
        else if (!is_playing)
        {
            footsteps.Play();
            is_playing = true;
        }
    }
}
