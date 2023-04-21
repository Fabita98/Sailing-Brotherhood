using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody belongingShip; 
    public float speed = 12f; 
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    // Start is called before the first frame update
    public Transform ship; // You put ship transform here
    private Quaternion rotOffset;
    private Vector3 posOffset;
 
    void Start()
    {
        posOffset = ship.position;
        rotOffset = ship.rotation;
    }



    void FixedUpdate()
    {
        if (ship != null)
        {
            Vector3 rotDiff = ship.rotation.eulerAngles - rotOffset.eulerAngles;
            Vector3 posDiff = ship.position - posOffset;
            Vector3 rotChange = (Quaternion.Euler(rotDiff) * (transform.position - ship.position))
                - (transform.position - ship.position);

            transform.position += posDiff + rotChange;
            if (Mathf.Abs(rotDiff.y) * 10 > 0)
                transform.rotation *= Quaternion.Euler(0, rotDiff.y, 0);
            
            
        }
        
    }
}
