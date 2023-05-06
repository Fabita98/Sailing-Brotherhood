using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMovement : MonoBehaviour
{
    public Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0f, 0f, -0.5f);
    }

    // Update is called once per frame
    void Update()
    { 
        gameObject.transform.position += velocity; 
    }
}
