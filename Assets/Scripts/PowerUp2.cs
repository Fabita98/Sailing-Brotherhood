using Crest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp2 : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    public GameObject barrel;
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
            GameObject newObject = Instantiate(barrel,other.transform.position,Quaternion.identity);
            newObject.transform.parent = other.transform;
        }
    }

}
