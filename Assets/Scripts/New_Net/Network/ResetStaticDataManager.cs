using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    private void Awake()
    {   //check player assignements for 1 ship first
        //OnBoardBehaviour.ResetStaticData(); 
        PlayerMovementNet.ResetStaticData();        
    }

}