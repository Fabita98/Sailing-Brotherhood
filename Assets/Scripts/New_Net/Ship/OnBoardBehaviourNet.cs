using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnBoardBehaviourNet : NetworkBehaviour
{
    public static event EventHandler OnSpawnedShip;
    public static event EventHandler OnPlayerAttached;
    public static event EventHandler OnListAddition;
    //OnListAddition?.Invoke(this, EventArgs.Empty);
    public NetworkVariable<Vector3> syncVel = new();
    
    public static OnBoardBehaviourNet LocalInstance { get; private set; }

    // How can you start an in-scene placed NetworkObject as de-spawned when the scene is first loaded (that is, its first spawn)?:
    // https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/inscene-placed-networkobjects/#spawning-and-de-spawning

    //Attached PlayerObject components
    [Header("Attached PlayerMovementInstance")]
    public List<GameObject> CrewmatesList;
    public GameObject fakeRespawn;

    //Rotation calculations
    Vector3 prevEulerAngles;
    Vector3 deltaEulerAngles;

    //Ship
    [Header("Ship components")]
    Rigidbody shipRb;

    //Here for operations order for in-scene NetObjects: https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/inscene-placed-networkobjects/
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnSpawnedShip?.Invoke(this, EventArgs.Empty);

        LocalInstance = this;

        //Init
        shipRb = GetComponent<Rigidbody>();
        syncVel.Value = shipRb.velocity; 
        prevEulerAngles = new Vector3(0, 0, 0);

        if (CrewmatesList == null) CrewmatesList = new List<GameObject>();        
    }

    void Update()
    {
        //Using euler angles
        deltaEulerAngles = (prevEulerAngles - transform.localEulerAngles);
        
        //Adjust crewmates rotation
        try
        {
            BringCrewmateOnBoard();
            if (CrewmatesList.Count != 0)
            {
                CrewmatesList.ForEach(delegate (GameObject p)
                {
                    p.transform.RotateAround(transform.position, Vector3.up, -1 * deltaEulerAngles.y);
                    p.transform.rotation = Quaternion.Euler(0, p.transform.localEulerAngles.y, 0);
                });
            }                      
        }
        catch {Debug.Log(" Failed to adapt Player rotation to ship"); }

        prevEulerAngles = transform.localEulerAngles;
    }

    private void FixedUpdate()
    {
        if (IsServer) syncVel.Value = shipRb.velocity;
        try
        {
            if (CrewmatesList.Count != 0)
            {
                CrewmatesList.ForEach(delegate (GameObject p)
                {
                    if (IsHost)
                    {
                        p.transform.GetComponent<Rigidbody>().AddForce(syncVel.Value.x, syncVel.Value.y, syncVel.Value.z, ForceMode.VelocityChange);
                    } 
                    else if (!IsHost) {
                        p.transform.GetComponent<Rigidbody>().AddForce(syncVel.Value.x, 0f, syncVel.Value.z + p.GetComponent<PlayerMovementNet>().delta, ForceMode.VelocityChange);
                    }
                });
            }                       
        }
        catch { Debug.Log("Movement player on-deck adjust failed"); }
    }

    public void AddToCrewList(GameObject playerToAdd)
    {
        OnListAddition?.Invoke(this, EventArgs.Empty);
        Debug.Log("Il playerToAdd attaccato via trigger è: " + playerToAdd.name);
        if (CrewmatesList.Count < 2)
        {
            CrewmatesList.Add(playerToAdd);
            Debug.Log("This ship has " + CrewmatesList.Count + " crewmates");
        }
        else return;
    }
    private void BringCrewmateOnBoard()
    {
        if (CrewmatesList.Count != 0)
        {
            CrewmatesList.ForEach(delegate (GameObject p)
            {
                if (p.transform.position.y < 25.79609) p.transform.position = fakeRespawn.transform.position;
            });
        }
    }
    public static void ResetStaticData()
    {
        OnSpawnedShip = null;
    }    
}
