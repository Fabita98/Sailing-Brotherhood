using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public Object Gold;
    public int countdownTime=1;
    // Start is called before the first frame update
    IEnumerator CountdownStart()
    {
        while (countdownTime > 0)
        {
            
            yield return new WaitForSeconds(1f);
            countdownTime--;
        } 
    }
    void Start()
    {
       StartCoroutine(CountdownStart()); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
