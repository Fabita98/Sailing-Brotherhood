using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnBoardBehaviour : MonoBehaviour
{
    //Attached PlayerObject components
    public GameObject attachedObject;
    public Rigidbody attachedRb;
        
    //Rotation calculations
    Vector3 prevEulerAngles;
    Vector3 deltaEulerAngles;        

    //Ship RB
    public Rigidbody shipRb;

    void Start() {
        //Init
        prevEulerAngles = new Vector3(0,0,0);
        // AttachPlayerObj
        Attach(attachedObject);
        
        //Detach when respawning
        //GameObject.Find("UI").GetComponent<PauseMenu>().onRespawn += unAttach;
    }

    void Update() {
        
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
        
    public void Attach(GameObject obj) {
        attachedObject = obj;
        attachedRb = attachedObject.GetComponent<Rigidbody>();
        //attachedObject.GetComponent(OnCollisionEnter) += checkDetach;
    }   
    
}
