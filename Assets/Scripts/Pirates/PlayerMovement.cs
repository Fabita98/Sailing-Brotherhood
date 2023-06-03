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
    public Animator player_an;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player_an = GetComponent<Animator>();
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
        if (playerHorizontalInput == 0 && playerVerticalInput == 0) {
            player_an.SetBool("iswalking", false);
            player_an.SetBool("iswalking_back", false);
            player_an.SetBool("iswalking_right", false);
            player_an.SetBool("iswalking_left", false);
        }
        else if (playerHorizontalInput > 0 && playerVerticalInput==0)
        {
            player_an.SetBool("iswalking_right", true);
            player_an.SetBool("iswalking", false);
            player_an.SetBool("iswalking_back", false);
            player_an.SetBool("iswalking_left", false);
        }
        else if (playerHorizontalInput < 0 && playerVerticalInput == 0)
        {
            player_an.SetBool("iswalking_right", false);
            player_an.SetBool("iswalking", false);
            player_an.SetBool("iswalking_back", false);
            player_an.SetBool("iswalking_left", true);
        }
        else if (playerVerticalInput < 0)
        {
            player_an.SetBool("iswalking_back", true);
            player_an.SetBool("iswalking", false);
            player_an.SetBool("iswalking_right", false);
            player_an.SetBool("iswalking_left", false);
        }
        else if (playerVerticalInput > 0) {
            player_an.SetBool("iswalking", true); 
            player_an.SetBool("iswalking_back", false);
            player_an.SetBool("iswalking_right", false);
            player_an.SetBool("iswalking_left", false);
        }
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
