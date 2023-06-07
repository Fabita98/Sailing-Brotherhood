using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannons : MonoBehaviour
{
    public GameObject pirata;
    public GameObject cannonBall;
    public Button button;
    public Camera cam;

    public Text mirino;
    private int cont;
    // Start is called before the first frame update
    void Start()
    {
        cont = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(pirata.transform.position, transform.position);
        //Debug.Log("distanza" + dist);

        if (dist < 4)
        {

            button.gameObject.SetActive(true);
            if (Input.GetKeyDown("space")&&cont==0)
            {
                mirino.text = "+";
                //cambiare visuale camera
                //cam.transform
                cam.transform.position = transform.position;
                cont = 1;
            }

            else if (Input.GetKeyDown("space")&&cont==1)
            {
                mirino.text = "";
                //cambiare visuale camera
                //cam.transform
                cam.transform.position = pirata.transform.position;
                cont = 0;
            }

        }
        else
        {
            button.gameObject.SetActive(false);
        }

    }
}
