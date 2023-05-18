using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public new Camera camera;

    public float forceAmount;
    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
        
    void Update()
    {
        if (Input.GetKey("a"))
        {
            rb.AddForce(transform.right * - forceAmount, ForceMode.Impulse);
        }
        if (Input.GetKeyUp("a"))
        {
            rb.AddForce(transform.right * forceAmount, ForceMode.VelocityChange);
        }

        if (Input.GetKey("d"))
        {
            rb.AddForce(transform.right * forceAmount, ForceMode.Impulse);
        }
        if (Input.GetKeyUp("d"))
        {
            rb.AddForce(transform.right * - forceAmount, ForceMode.VelocityChange);
        }

        if (Input.GetKey("w"))
        {
            rb.AddForce(transform.forward * forceAmount, ForceMode.Impulse);
        }
        if (Input.GetKeyUp("w"))
        {
            rb.AddForce(transform.forward * - forceAmount, ForceMode.VelocityChange);
        }

        if (Input.GetKey("s"))
        {
            rb.AddForce(transform.forward * - forceAmount, ForceMode.Impulse);
        }
        if (Input.GetKeyUp("s"))
        {
            rb.AddForce(transform.forward * forceAmount, ForceMode.VelocityChange);
        }
    }

}