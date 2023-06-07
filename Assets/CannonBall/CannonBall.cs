using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float speed = 3;
    public Vector3 direction=Vector3.forward;

    public void setDirection(Vector3 dir)
    {
        direction = dir;
    }

    void FixedUpdate()
    {
        transform.position += direction * speed * Time.deltaTime;
        speed += 1f;

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Destroy(this.gameObject);
    }
}
