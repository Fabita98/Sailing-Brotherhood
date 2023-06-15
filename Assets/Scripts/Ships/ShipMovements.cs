using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovements : MonoBehaviour
{
    public float destra,sinistra;
    private Quaternion startRotation;
    //la startspeed inizialmente e 0, aumenta quando si levera l'ancora
    public int startSpeed;
    void Start()
    {
        startRotation = transform.rotation;
    }
    void Update()
    {
        //serve per far ruotare (rollare) la nave
        float f = Mathf.Sin(Time.time * destra) * sinistra;
        transform.rotation = startRotation * Quaternion.AngleAxis(f, Vector3.forward);

        //float f = Mathf.PingPong(Time.time, sinistra) - destra;
        //transform.rotation = startRotation * Quaternion.AngleAxis(f, Vector3.forward);

        //serve a far ruotare (pitch) la nave in avanti
        float avanti = Mathf.Sin(Time.time * destra) * sinistra;
        transform.rotation = startRotation * Quaternion.AngleAxis(avanti, Vector3.left);

        //una volta levata l'ancora deve partire la nave
        transform.Translate(Vector3.forward*Time.deltaTime);
    }
}
