using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera camera;    
    public float speed = 1;
    public Rigidbody rb;
    public Vector3 inputMovement;    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
    }


    void FixedUpdate()
    {
       //Rb movement here
       MovePlayerRelativeToCamera();
    }

    //Rb.velocity movement according to the pressed keys and their relative axes
    void MovePlayerRelativeToCamera()
    {
        float playerHorizontalInput = Input.GetAxis("Horizontal");
        float playerVerticalInput = Input.GetAxis("Vertical");
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;
        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 rightRelativeVerticalInput = playerHorizontalInput * right;
        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;
        //rb.AddForce(cameraRelativeMovement * speed, ForceMode.VelocityChange);
        rb.velocity= cameraRelativeMovement* speed;
    }
}
