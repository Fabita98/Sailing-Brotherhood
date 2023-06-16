using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    // Serialized fields for ships and players
    public List<OnBoardBehaviourNet> onBoardBehavioursList;
    
    // Private static instance variable
    private static ShipManager instance;

    // Public property to access the singleton instance
    public static ShipManager Instance
    {
        get
        {
            // Create a new instance if it doesn't exist
            if (instance == null)
            {
                // Check if an instance of ShipManager already exists in the scene
                instance = FindObjectOfType<ShipManager>();

                // If not, create a new ShipManager object
                if (instance == null)
                {
                    GameObject obj = new GameObject("ShipManager");
                    instance = obj.AddComponent<ShipManager>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        onBoardBehavioursList = new List<OnBoardBehaviourNet>();
        OnBoardBehaviourNet.OnSpawnedShip += HandleSpawnedShip;

    }
    public void AddToCrewList(GameObject playerToAdd)
    {
        for (int i = 0; i < onBoardBehavioursList.Count; i++)
        {
            if (onBoardBehavioursList[i].CrewmatesList.Count < 2)
            {
                onBoardBehavioursList[i].AddToCrewList(playerToAdd);
                Debug.Log($"Player attached via ShipManager to ship {i}");
                return;
            }
        }        
        Debug.Log("No available ship to attach the player to.");
    }
    private void HandleSpawnedShip(object sender, EventArgs e)
    {
        Debug.Log("Ship spawned!");
    }
}
