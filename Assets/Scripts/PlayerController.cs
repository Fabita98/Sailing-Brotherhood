using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -1f;
    public float jump = 5f;
    public GameObject pirate;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Rigidbody belongingShip;
    private float x, z;
    private Vector3 move;

    public Rigidbody rb_player;
    public Animator an_player;
    public bool driving = false;
    private Vector3 velocity;
    private bool isGrounded=false;

    // Update is called once per frame
    public void Start()
    {
        rb_player.maxDepenetrationVelocity=0f;
        //belongingShip.maxDepenetrationVelocity = 0f;
    }

    private void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        move = transform.right * x * speed + transform.forward * z * speed;
        if (driving)
        {
            move = new Vector3(0, 0, 0);
        }
        move.x += belongingShip.velocity.x;
        move.z += belongingShip.velocity.z;
        controller.Move(move * Time.deltaTime);
        
        
        velocity.y = gravity;
        if (controller.isGrounded)
        {
            velocity.y = -0.1f;
        }

        controller.Move(velocity * Time.deltaTime);
        an_player.SetFloat("speed", move.magnitude);

    }

    private void FixedUpdate()
    {   
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);


    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("collision with wall");
        //move -= hit.normal * Vector3.Dot(move, hit.normal);
    }

}
   

