using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManagerStatic : MonoBehaviour
{
    public GameObject world;
    public WorldMovement movement;
    public Vector3 WorldVel;
    bool active = false;
    public GameObject driver;
    private PlayerController PirateMovement;
    public GameObject wheel;
    private float x;
    private bool left, right;
    // Start is called before the first frame update
    void Start()
    {
        movement = world.GetComponent<WorldMovement>();
        WorldVel = movement.velocity;
        PirateMovement = driver.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider other)
    {
        
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown("e"))
            {
                Debug.Log("timone");
                active = !active;
            }
            if (active)
            {
                PirateMovement.driving = true;
                x = Input.GetAxis("Horizontal");
                if (x > 0)
                {
                    right = true;
                    left = false;
                }
                if (x < 0){ 
                left = true; right = false;
                }
                wheel.gameObject.transform.Rotate(0, 0, -x);
            }
            else PirateMovement.driving = false;
        }
    }
    void Update()
    {
        if (active) wheel.gameObject.transform.Rotate(0, 0, x);

        if (right) { movement.velocity.x -= x*Time.deltaTime; }

        if (left) { movement.velocity.x -= x * Time.deltaTime; }
    }
}
