using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jump = 5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Rigidbody belongingShip;

    public Rigidbody rb_player;
    public Animator an_player;

    Vector3 velocity;
    private bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -8f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        //move += belongingShip.velocity;
        controller.Move(move * speed * Time.deltaTime);

        /*
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
            //an_player.SetBool("jump", true);
        } else an_player.SetBool("jump", false);
        */
        velocity.y += gravity * Time.deltaTime;


        controller.Move(velocity * Time.deltaTime);
    } 
    
    private void FixedUpdate()
    {
        an_player.SetFloat("speed", rb_player.velocity.magnitude);
    }
}
   

