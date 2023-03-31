using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceSystem : MonoBehaviour
{
    public Text DistanceText;
    [Header("Ships")]
    public Transform T_Ship01;
    public Transform T_Ship02;
    public Object Ship01;
    public Object Ship02;
    [Header("Distances")]
    public float Ship01Dist;
    public float Ship02Dist;

    [Header("Placements")]
    public float First;
    public float Second;
    private string NameFirst;
    private string NameSecond;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ship01Dist = Vector3.Distance(transform.position, T_Ship01.position);
        Ship02Dist = Vector3.Distance(transform.position, T_Ship02.position);
        First = Mathf.Min(Ship01Dist, Ship02Dist);
        Second = Mathf.Max(Ship01Dist, Ship02Dist);

        if (First == Ship01Dist)
        {
            NameFirst = Ship01.name;
            NameSecond = Ship02.name;
        }
        else
        {
            NameFirst = Ship02.name;
            NameSecond = Ship01.name;
        }

        DistanceText.text = NameFirst +"  "+ (int)First + "m\r\n\r\n"+ NameSecond +"  "+ (int)Second +"m";

        
    }
}
