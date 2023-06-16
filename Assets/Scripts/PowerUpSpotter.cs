using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpotter : MonoBehaviour
{
    public GameObject parrot;
    private ParrotAI pAI;
    // Start is called before the first frame update
    void Start()
    {
        pAI = parrot.GetComponent<ParrotAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUp") pAI.powerup_ahead = true; else pAI.powerup_ahead = false; 
    }
}
