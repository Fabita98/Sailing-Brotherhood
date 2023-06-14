using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPressed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ButtonPlay);
    }

    // Update is called once per frame
    public void ButtonPlay()
    {
        PlayRandomHello.playRnd.PlayButtonHello();
    }
}
