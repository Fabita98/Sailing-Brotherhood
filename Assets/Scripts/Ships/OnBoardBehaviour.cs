using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class OnBoardBehaviour : NetworkBehaviour
{
    //Attached PlayerObject components
    [Header("Attach Player GameObj")]
    public GameObject attachedObject;
    private Rigidbody attachedRb;
        
    //Rotation calculations
    Vector3 prevEulerAngles;
    Vector3 deltaEulerAngles;        

    //Ship
    private Rigidbody shipRb;
    [HideInInspector]
    public GameObject shipObj;

    private void Awake()
    {
        
    }
    void Start() {
        //Init
        prevEulerAngles = new Vector3(0,0,0);
        
        shipRb = GetComponent<Rigidbody>();
        shipObj = GetComponent<GameObject>();
        
        //Detach when respawning
        //GameObject.Find("UI").GetComponent<PauseMenu>().onRespawn += unAttach;
    }

    void Update() {
        // AttachPlayerObj and ShipRb
        Attach(attachedObject);
        //Using euler angles
        deltaEulerAngles = (prevEulerAngles - transform.localEulerAngles);

        if (attachedObject != null) {

            //Adjust rotation
            attachedObject.transform.RotateAround(transform.position, Vector3.up, -1 * deltaEulerAngles.y);
            attachedObject.transform.rotation = Quaternion.Euler(0,attachedObject.transform.localEulerAngles.y, 0);
        }
        prevEulerAngles = transform.localEulerAngles;       
    }

    private void FixedUpdate() {
        //Adjust velocity
        if (attachedObject != null)
        {
            attachedRb.AddForce(shipRb.velocity.x, shipRb.velocity.y, shipRb.velocity.z, ForceMode.VelocityChange);
        }
    }

    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.tag == "Player" && attachedObject == null)
    //    {
    //        Attach(other.gameObject);
    //    }
    //}
        
    private void Attach(GameObject obj) {
        attachedObject = obj;
        attachedRb = obj.GetComponent<Rigidbody>();
        //attachedObject.GetComponent(OnCollisionEnter) += checkDetach;
    }   
    
}
