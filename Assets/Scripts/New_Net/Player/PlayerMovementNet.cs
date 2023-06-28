using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementNet : NetworkBehaviour
{
    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public float KeepPlayerOnGroundForce;
    private float playerHeight;
    private bool grounded;
    
    [Header("Movement")]
    private bool lockMovement = false;
    public float speed;
    //[SerializeField] private Camera playerCameraPrefab;
    private Camera playerCamera;
    private GameObject gameObjectPlayerCamera;
    [SerializeField] 
    private List<float> clientDeltas;
    public float delta;

    public Vector3 cameraRelativeMovement;
    public Rigidbody rb;
    public Animator player_an;

    [Header("Slope/Stairs Handling")]
    public float maxSlopeAngle;
    public float onSlopeForceMultiplier;
    public float pushDownForceOnSlope;
    public LayerMask whatIsStairs;
    private RaycastHit slopeHit;
    private bool onStairs;

    public static PlayerMovementNet LocalInstance { get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;

    [SerializeField] private List<Vector3> spawnPositionList;

    public bool driving;

    public override void OnNetworkSpawn()
    {

        playerCamera = GetComponentInChildren<Camera>();
        gameObjectPlayerCamera = playerCamera.gameObject;
        if (IsLocalPlayer)
        {
            LocalInstance = this;

            gameObjectPlayerCamera.SetActive(true);
            //playerCamera.enabled = true;          
        }
        else
        {
            gameObjectPlayerCamera.SetActive(false); 
            return; } // Disabilita la camera per gli altri giocatori
        
        playerCamera.tag = "MainCamera";

        rb = GetComponent<Rigidbody>();
        player_an = GetComponent<Animator>();
        playerHeight = GetComponent<CapsuleCollider>().height;

        Debug.Log("Camera: " +  playerCamera);        

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (this != null) {
            Debug.Log("A new Player instance with ID " + GetInstanceID() +
            " exists and its OwnerClientId is: " + OwnerClientId);
        }
        
        transform.position = spawnPositionList[SailingBrotheroodLobby.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        delta = clientDeltas[SailingBrotheroodLobby.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

        if (IsServer /*&& IsLocalPlayer*/)
        {
            //Camera.main.enabled = true;
            //rb = GetComponent<Rigidbody>();
            //player_an = GetComponent<Animator>();
            //playerHeight = GetComponent<CapsuleCollider>().height;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
        Debug.Log("THIS Player LocalInstance ID: " + LocalInstance.GetInstanceID() + " exists" );
        PlayerData playerData = SailingBrotheroodLobby.Instance.GetPlayerDataFromClientId(OwnerClientId);
    }

    private void Update()
    {   //messo qua per farlo muovere solo dal localplayer

        /*if (Input.GetKeyDown(KeyCode.C))
        {
            playerCamera.enabled=!playerCamera.enabled;
        }*/

        if (!IsLocalPlayer)
        {
            return;
        }

        CollisionDetection();
        HandleAnimation();
    }

    void FixedUpdate()
    {   //messo qua per farlo muovere solo dal localplayer
        if (!IsLocalPlayer)
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
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
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
        if (IsLocalPlayer)
        {
            if (LocalInstance.driving) {} 
                else if (playerHorizontalInput == 0 && playerVerticalInput == 0)
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

