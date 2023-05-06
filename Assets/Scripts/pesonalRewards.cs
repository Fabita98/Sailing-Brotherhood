using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class pesonalRewards : MonoBehaviour
{
    public int lenght;// Start is called before the first frame update
    public TextMeshProUGUI racelenght;
    void Start()
    {
        lenght = DistanceSystem.raceLenght;
        racelenght.text = "Race distance: " + lenght.ToString();
    }

    
}
