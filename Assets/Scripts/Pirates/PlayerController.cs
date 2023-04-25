using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jump = 5f;
    [SerializeField] private float positionRange = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Rigidbody belongingShip;

    public Rigidbody rb_player;
    public Animator an_player;

    Vector3 velocity;
    private bool isGrounded;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        UpdateSpawnPosition();
    }

    private void UpdateSpawnPosition()
    {
        transform.position = new Vector3(Random.Range(belongingShip.position.x + positionRange, belongingShip.position.x - positionRange), 7.5f, Random.Range(belongingShip.position.z + positionRange, belongingShip.position.z - positionRange));
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -8f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        //move += belongingShip.velocity;
        controller.Move(speed * Time.deltaTime * move);

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


