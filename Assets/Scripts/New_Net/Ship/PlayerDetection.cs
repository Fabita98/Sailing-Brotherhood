using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public GameObject myShip;
    private int count;
    
    public void OnTriggerEnter(Collider other)
    {
        if (myShip == null)
        {
            Debug.LogError("OnBoardBehaviour.LocalInstance is null!");
            return;
        }

        if (myShip.GetComponent<OnBoardBehaviourNet>().CrewmatesList.Count == 2)
        {
            Debug.Log("Crewmates list is full. Cannot add player.");
            return;
        }

        if (other.gameObject.CompareTag("Player") && count == 0)
        {
            GameObject playerGO = other.gameObject/*FindParentObjectWithTag("Player")*/;

            if (playerGO != null)
            {
                //myShip.GetComponent<OnBoardBehaviourNet>().attachedPlayer = playerGO;
                myShip.GetComponent<OnBoardBehaviourNet>().AddToCrewList(playerGO);
                count++;
                //Debug.Log("Player attached via CollisionTrigger");

                if (myShip.GetComponent<OnBoardBehaviourNet>().CrewmatesList.Count == 2)
                {
                    Debug.Log("Crewmates list is full. Cannot add player.");
                    return;
                }
            }
        }
    }

    private GameObject FindParentObjectWithTag(string tag)
    {
        Transform parentTransform = transform.Find(tag);

        if (parentTransform != null)
        {
            return parentTransform.gameObject;
        }

        return null;
    }            

}
