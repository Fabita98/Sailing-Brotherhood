using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public float KeepPlayerOnGroundForce;
    private float playerHeight;
    private bool grounded;    

    [Header("Movement")]
    private bool lockMovement = false;
    public float speed;
    public Camera PlayerCamera;
    public Vector3 cameraRelativeMovement;
    private Rigidbody rb;
    private Animator player_an;    

    [Header("Slope/Stairs Handling")]
    public float maxSlopeAngle; 
    public float onSlopeForceMultiplier;
    public float pushDownForceOnSlope;
    public LayerMask whatIsStairs;
    private RaycastHit slopeHit;    
    private bool onStairs;
    
    public static PlayerMovement LocalInstance { get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;

    [SerializeField] private List<Vector3> spawnPositionList;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionList[SailingBrotheroodLobby.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerData playerData = SailingBrotheroodLobby.Instance.GetPlayerDataFromClientId(OwnerClientId);
        rb = GetComponent<Rigidbody>();
        player_an = GetComponent<Animator>();
        playerHeight = GetComponent<CapsuleCollider>().height;
    }
     
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        CollisionDetection();
        HandleAnimation();                
    }
    
    void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

       // PlayerRb movement here
       MovePlayerRelativeToCamera();
       KeepPlayerOnGround();
    }

    // Rb.velocity movement according to the pressed keys and their relative axes
    void MovePlayerRelativeToCamera()
    {
        if (lockMovement)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        float playerHorizontalInput = Input.GetAxis("Horizontal");
        float playerVerticalInput = Input.GetAxis("Vertical");
        Vector3 forward = PlayerCamera.transform.forward;
        Vector3 right = PlayerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;
        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 rightRelativeVerticalInput = playerHorizontalInput * right;
        cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;
        // Diagonal movement normalization
        cameraRelativeMovement.Normalize();
        rb.velocity = cameraRelativeMovement * speed;

        // On slope relative movement
        if (OnSlopeDetection())
        {
            rb.useGravity = false;
            rb.velocity = onSlopeForceMultiplier * speed * GetSlopeMoveDirection();

            // OnSlope velocity and speed limit 
            if (rb.velocity.magnitude > speed)
                rb.velocity = rb.velocity.normalized * speed;

            // Bumping movement fix
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * pushDownForceOnSlope, ForceMode.Force);
        }
    }

    private bool OnSlopeDetection()
    {
        if (onStairs)
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(cameraRelativeMovement, slopeHit.normal).normalized;
    }

    private void HandleAnimation()
    {   // Animator relative code
        float playerHorizontalInput = Input.GetAxis("Horizontal");
        float playerVerticalInput = Input.GetAxis("Vertical");
        if (playerHorizontalInput == 0 && playerVerticalInput == 0)
        {
            player_an.SetBool("iswalking", false);
            player_an.SetBool("iswalking_back", false);
            player_an.SetBool("iswalking_right", false);
            player_an.SetBool("iswalking_left", false);
        }
        else if (playerHorizontalInput > 0 && playerVerticalInput == 0)
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
        else if (playerVerticalInput > 0)
        {
            player_an.SetBool("iswalking", true);
            player_an.SetBool("iswalking_back", false);
            player_an.SetBool("iswalking_right", false);
            player_an.SetBool("iswalking_left", false);
        }
    }

    private void KeepPlayerOnGround()
    {
        if (!grounded || !onStairs)
            rb.AddForce(Vector3.down * KeepPlayerOnGroundForce, ForceMode.Force);
    }

    public void CollisionDetection()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.5f, whatIsGround);
        // stairs/slope check 
        onStairs = Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, whatIsStairs);
    }

    public void LockMovement()
    {
        lockMovement = true;
    }

    public void UnlockMovement()
    {
        lockMovement = false;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //if (clientId == OwnerClientId && HasKitchenObject())
        //{
        //    KitchenObject.DestroyKitchenObject(GetKitchenObject());
        //}
        //
        //Now only debugs a player disconnected
        Debug.Log("Player disconnected!");
    }

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
}

