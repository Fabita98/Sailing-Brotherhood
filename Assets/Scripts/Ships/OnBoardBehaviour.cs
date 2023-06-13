using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class OnBoardBehaviour : NetworkBehaviour
{
    public static event EventHandler OnPlayerAssigned;

    public static OnBoardBehaviour LocalInstance { get; private set; }
    public bool StartDespawned;
    private bool m_HasStartedDespawned;

    // How can you start an in-scene placed NetworkObject as de-spawned when the scene is first loaded (that is, its first spawn)?:
    // https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/inscene-placed-networkobjects/#spawning-and-de-spawning
    //public override void OnNetworkDespawn()
    //{
    //    gameObject.SetActive(false);
    //    base.OnNetworkDespawn();
    //}

    //public void Spawn(bool destroyWithScene)
    //{
    //    if (IsServer && !IsSpawned)
    //    {
    //        gameObject.SetActive(true);
    //        NetworkObject.Spawn(destroyWithScene);
    //    }
    //}

    //Attached PlayerObject components
    [Header("Attached PlayerMovementInstance")]
    public List<PlayerMovement> attachedPlayersList = new List<PlayerMovement>(new PlayerMovement[2]);

    //Rotation calculations
    Vector3 prevEulerAngles;
    Vector3 deltaEulerAngles;

    //Ship
    [Header("Ship components")]
    Rigidbody shipRb;

    //Here for operations order for in-scene NetObjects: https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/inscene-placed-networkobjects/

    private void Awake()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }        
    }

    void Start()
    {
        //Init
        shipRb = GetComponent<Rigidbody>();
        prevEulerAngles = new Vector3(0, 0, 0);
    }

    //public override void OnNetworkSpawn()
    //{
    //    if (IsServer && StartDespawned && !m_HasStartedDespawned)
    //    {
    //        m_HasStartedDespawned = true;
    //        NetworkObject.Despawn(false);
    //        LocalInstance.AddPlayerInShipCrewList(PlayerMovement.LocalInstance);
    //        foreach (PlayerMovement player in attachedPlayersList) { Debug.Log("Player instance OwnerClientID: " + player.OwnerClientId + " NetID: " + player.NetworkObjectId); }
    //    }
    //    base.OnNetworkSpawn();
    //}

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        //Using euler angles
        deltaEulerAngles = (prevEulerAngles - transform.localEulerAngles);
        
        //Adjust players' rotation relative to their ships
        try
        {            
            if (attachedPlayersList.Count > 0)
            {
                foreach (PlayerMovement player in attachedPlayersList)
                {
                    if (player != null)
                    {
                        player.transform.RotateAround(transform.position, Vector3.up, -1 * deltaEulerAngles.y);
                        player.transform.rotation = Quaternion.Euler(0, player.transform.localEulerAngles.y, 0);
                    }
                }
            }
        } catch (Exception e) {Debug.Log(e + " Failed to assign Players to  ship"); }

        prevEulerAngles = transform.localEulerAngles;
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        try
        {
            //Adjust velocity
            if (attachedPlayersList.Count > 0)
                foreach (PlayerMovement player in attachedPlayersList)
                {
                    {
                        player.GetComponent<Rigidbody>().AddForce(shipRb.velocity.x, shipRb.velocity.y, shipRb.velocity.z, ForceMode.VelocityChange);
                    }
                }
        } catch (Exception e) { Debug.Log(e) ; }
    }    

    public void AddPlayerInShipCrewList(PlayerMovement playerToAdd)
    {        
            if (playerToAdd.IsSpawned && attachedPlayersList.Count < 2)
            {
                attachedPlayersList.Add(playerToAdd);
                OnPlayerAssigned?.Invoke(this, EventArgs.Empty);
                Debug.Log("Player instance: " + playerToAdd.name + " assigned to the ship " + LocalInstance.name);
            }
            else { Debug.Log("NO SPAWNED Player instance: " + playerToAdd.name + " and not assigned to any ship"); }
    }

    public static void ResetStaticData()
    {
        if ( LocalInstance.attachedPlayersList.Count == 2)
        OnPlayerAssigned = null;
    }
}
