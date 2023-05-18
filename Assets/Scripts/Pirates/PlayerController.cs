//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    public CharacterController controller;

//    [SerializeField] private float speed = 12f;
//    public float gravity = -9.81f;
//    public float jump = 5f;
//    public GameObject pirate;
//    public Transform groundCheck;
//    public float groundDistance = 0.4f;
//    public LayerMask groundMask;
//    public GameObject belongingShip;
//    x = Input.GetAxis("Horizontal");
//    z = Input.GetAxis("Vertical");    
//    private Vector3 move;

//    public Rigidbody rb_player;
//    public Animator an_player;
//    public bool driving = false;
//    Vector3 velocity;
//    private bool isGrounded = false;

//    // Update is called once per frame
//    public void Start()
//    {
//        rb_player.maxDepenetrationVelocity = 0f;
//        //belongingShip.maxDepenetrationVelocity = 0f;
//    }

//    private void Update()
//    {
        
//        move = transform.right * x * speed + transform.forward * z * speed;
//        if (driving)
//        {
//            move = new Vector3(0, 0, 0);
//        }

//        float x = Input.GetAxis("Horizontal");
//        float z = Input.GetAxis("Vertical");

//        Vector3 move = transform.right * x * speed + transform.forward * z * speed;
//        move.x += belongingShip.velocity.x;
//        move.z += belongingShip.velocity.z;
//        //move = belongingShip.transform.position.x + belongingShip.transform.position.z;
//        controller.Move( Time.deltaTime * move);

//        /*
//        if(Input.GetButtonDown("Jump") && isGrounded)
//        {
//            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
//            //an_player.SetBool("jump", true);
//        } else an_player.SetBool("jump", false);
//        */
//        velocity.y += gravity * Time.deltaTime;
//        if (controller.isGrounded)
//        {
//            velocity.y = -0.1f;
//        }
//        controller.Move(velocity * Time.deltaTime);
//        an_player.SetFloat("speed", move.magnitude);

//    }

//    private void FixedUpdate()
//    {
//        //an_player.SetFloat("speed", rb_player.velocity.magnitude);
//    }
//}