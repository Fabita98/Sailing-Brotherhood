using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnBoardBehaviour : MonoBehaviour
{

    //Attached object components
    public GameObject attachedObject;
    public Rigidbody attachedRb;
        
    //Rotation calculations
    Vector3 prevEulerAngles;
    Vector3 deltaEulerAngles;

    public bool updateAttachedObject;

    //Carrier RB
    public Rigidbody rb;

    void Start() {
        //Init
        prevEulerAngles = new Vector3(0,0,0);
        updateAttachedObject = true;
        
        //Detach when respawning
        //GameObject.Find("UI").GetComponent<PauseMenu>().onRespawn += unAttach;
    }

    void Update() {
        
        //Using euler angles
        deltaEulerAngles = (prevEulerAngles - transform.localEulerAngles);


        if (attachedObject != null && updateAttachedObject) {

            //Adjust rotation
            attachedObject.transform.RotateAround(transform.position, Vector3.up, -1 * deltaEulerAngles.y);
            attachedObject.transform.rotation = Quaternion.Euler(0,attachedObject.transform.localEulerAngles.y, 0);
        }

        prevEulerAngles = transform.localEulerAngles;

        if (attachedObject != null) {
            unAttach();
        }

    }

    private void FixedUpdate() {
        
        //Adjust velocity: check the ForceMode to a more consistent movement
        if (attachedObject != null && updateAttachedObject)
        {
            attachedRb.AddForce(rb.velocity.x, rb.velocity.y, rb.velocity.z, ForceMode.VelocityChange);            
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && attachedObject == null)
        {
            attach(other.gameObject);
        }
    }

    //When the player collides with an object, check if it is not the boat.  If it isnt, then detach
    void checkDetach(Collision other) {
        if (transform == null) {
            unAttach();
        }
        if (!other.transform.IsChildOf(transform) && other.gameObject.tag != "Player") {
            unAttach();
        }
    }

    public void attach(GameObject obj) {
        attachedObject = obj;
        attachedRb = attachedObject.GetComponent<Rigidbody>();
        //attachedObject.GetComponent(OnCollisionEnter) += checkDetach;
    }

    public void unAttach() {
        if (attachedObject != null) {
            //attachedObject.GetComponent<CollisionEvent>().onCollide -= checkDetach;
            attachedObject = null;
            attachedRb = null;
        }
    }

    public void OnDestroy() {
        unAttach();
    }
    
}
