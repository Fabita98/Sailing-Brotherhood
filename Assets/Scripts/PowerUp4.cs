using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp4 : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    private GameObject ship;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ship")
        {
            GameObject shipBody = other.transform.parent.gameObject;
            GameObject shipComponent = shipBody.transform.parent.gameObject;
            GameObject shipCompleted = shipComponent.transform.parent.gameObject;
            ship = shipCompleted.gameObject;
            GameObject smallCannon = shipCompleted.transform.Find("SmallCannonDetection").gameObject;
            SmallCannonTrigger smallCannonTrigger = smallCannon.GetComponent<SmallCannonTrigger>();
            smallCannonTrigger.addCannonBalls(1);
        }
    }
}
