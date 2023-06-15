using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (OnBoardBehaviourNet.LocalInstance == null)
        {
            Debug.LogError("OnBoardBehaviour.LocalInstance is null!");
            return;
        }

        if (OnBoardBehaviourNet.LocalInstance.CrewmatesList.Count >= 2)
        {
            Debug.Log("Crewmates list is full. Cannot add player.");
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            GameObject parentObject = other.gameObject/*FindParentObjectWithTag("Player")*/;

            if (parentObject != null)
            {
                OnBoardBehaviourNet.LocalInstance.attachedPlayer = parentObject;
                ShipManager.Instance.AddToCrewList(parentObject);
                Debug.Log("Player attached via CollisionTrigger");
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
