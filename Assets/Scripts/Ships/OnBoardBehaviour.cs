using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnBoardBehaviour : NetworkBehaviour
{
    public static event EventHandler OnSpawnedShip;
    public static event EventHandler OnPlayerAttached;
    public static event EventHandler OnListAddition;
    //OnListAddition?.Invoke(this, EventArgs.Empty);

    public OnBoardBehaviour LocalInstance { get; private set; }

    // How can you start an in-scene placed NetworkObject as de-spawned when the scene is first loaded (that is, its first spawn)?:
    // https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/inscene-placed-networkobjects/#spawning-and-de-spawning

    //Attached PlayerObject components
    [Header("Attached PlayerMovementInstance")]
    public List<GameObject> CrewmatesList;
    public GameObject attachedPlayer;
    public GameObject ship;

    //Rotation calculations
    Vector3 prevEulerAngles;
    Vector3 deltaEulerAngles;

    //Ship
    [Header("Ship components")]
    Rigidbody shipRb;

    //Here for operations order for in-scene NetObjects: https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/inscene-placed-networkobjects/
    private void Awake()
    {
        LocalInstance = this;
    }

    void Start()
    {
        //Init
        shipRb = GetComponent<Rigidbody>();
        prevEulerAngles = new Vector3(0, 0, 0);        
    }

    public override void OnNetworkSpawn()
    {
        OnSpawnedShip?.Invoke(this, EventArgs.Empty);

        if (LocalInstance != null) { Debug.Log("A Ship instance EXIST! and has NetId: " + LocalInstance.NetworkObjectId); }
        else { Debug.Log(" A Ship instance DO NOT exist! "); }       

        if(CrewmatesList.Count == 0) { Debug.Log(" Crewmates list is empty! "); }
    }

    void Update()
    {
        //Using euler angles
        deltaEulerAngles = (prevEulerAngles - transform.localEulerAngles);

        //Adjust crewmates rotation
        try
        {
            AddToCrewList(attachedPlayer);
            if (CrewmatesList.Count != 0)
            {
                CrewmatesList.ForEach(delegate (GameObject p)
                {
                    p.transform.RotateAround(transform.position, Vector3.up, -1 * deltaEulerAngles.y);
                    p.transform.rotation = Quaternion.Euler(0, p.transform.localEulerAngles.y, 0);
                });
            } else return;                       
        }
        catch (Exception e) {Debug.Log(e + " Failed to adapt Players-ship rotations"); }

        prevEulerAngles = transform.localEulerAngles;
    }

    private void FixedUpdate()
    {
        try
        {
            if (CrewmatesList.Count != 0)
            {
                CrewmatesList.ForEach(delegate (GameObject p)
                {
                    //Adjust crewmates velocity
                    p.transform.GetComponent<Rigidbody>().AddForce(shipRb.velocity.x, shipRb.velocity.y, shipRb.velocity.z, ForceMode.VelocityChange);
                });
            }
            else return;            
        }
        catch (Exception e) { Debug.Log(e) ; }
    }    
    
    public void AddToCrewList(GameObject playerToAdd)
    {
        OnListAddition?.Invoke(this, EventArgs.Empty);

        if (CrewmatesList.Count < 2 && attachedPlayer != null) 
        {
            CrewmatesList.Add(playerToAdd);
            Debug.Log("This ship has " + CrewmatesList.Count + " crewmates");
        }
    }    

    public static void ResetStaticData()
    {
        OnSpawnedShip = null;
    }    
}
