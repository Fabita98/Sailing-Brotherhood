using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Anchor : MonoBehaviour
{
    public GameObject pirata;
    public Button button;
    private int cont = 0;
    private int child=41;
    private float movementSpeed = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cont < 120)
        {
            float dist = Vector3.Distance(pirata.transform.position, transform.position);

            if (dist <= 13) { 
                button.gameObject.SetActive(true);
                if (Input.GetKeyDown("space"))
                {
                    cont++;
                    print("space key pressed "+cont+" times");
                    transform.position = transform.position + new Vector3(0,  movementSpeed , 0);                  
                    if (cont % 3 == 0) {
                        Destroy(transform.GetChild(child).gameObject);
                        child--;
                    }

                }
            }
            else {
                button.gameObject.SetActive(false);
            }
        }
        else { 
            button.gameObject.SetActive(false);
        }
    }
}
