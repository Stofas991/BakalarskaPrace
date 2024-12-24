/**
* File: Interactable.cs
* Author: Kryštof Glos
* Date Last Modified: 3.5.2024
* Description: This script defines the behavior of interactable objects in the game.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Reference to the health of the interactable object
    public UnitStats myHealth { get; private set; }

    public InteractableType type;

    // Interaction radius
    public float radius = 2f;

    // Flags to track focus and interaction status
    bool isFocus = false;
    public bool hasInteracted = false;
    public bool isEnemy = false;

    // List of players currently interacting with this object
    public List<Transform> playerList;

    void Update()
    {
        // Check if the object is focused and hasn't been interacted with yet (for non-enemy objects)
        if (isFocus && !hasInteracted && !isEnemy)
        {
            if (gameObject != null)
            {
                foreach (Transform player in playerList.ToList())
                {
                    if (player != null)
                    {
                        // Calculate distance between player and interactable object
                        float distance;
                        if (this != null)
                        {
                            distance = Vector3.Distance(player.position, transform.position);
                            // If player is within interaction radius, trigger interaction
                            if (distance <= radius)
                            {
                                Interact(player);
                            }
                        }
                    }
                }
            }
        }

        // Check if the object is focused and is an enemy
        if (isFocus && isEnemy)
        {
            if (transform != null)
            {
                foreach (Transform player in playerList.ToList())
                {
                    if (player == null)
                    {
                        // Remove null player references from the list
                        playerList.Remove(player);
                        continue;
                    }
                    // Calculate distance between player and interactable object
                    float distance = Vector3.Distance(player.position, transform.position);
                    // If player is within attack range, trigger interaction
                    if (distance <= player.GetComponent<UnitStats>().attackRange)
                    {
                        Interact(player);
                    }
                }
            }
        }
    }

    // Method to handle interaction with the object (to be overridden by derived classes)
    public virtual void Interact(Transform interactingPlayer)
    {
        // This method will be overridden by objects that use it
    }

    // Method to handle focus on the object
    public void OnFocused(Transform playerTransform)
    {
        // Reset interaction status and set focus to true
        hasInteracted = false;
        isFocus = true;

        UnitControlScript playerControlScript = playerTransform.GetComponent<UnitControlScript>();
        // Add the player to the list of interacting players if not already present
        if (!playerList.Contains(playerTransform))
            playerList.Add(playerTransform);

        switch(type)
        {
            case InteractableType.None:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Object has no interaction type set");
                Console.ResetColor();
                break;
            case InteractableType.Tree:
                playerControlScript.SetActivity(UnitControlScript.ActivityType.CuttingTrees);
                break;
            case InteractableType.Hill:
                playerControlScript.SetActivity(UnitControlScript.ActivityType.Digging);
                break;
            case InteractableType.Enemy:
                playerControlScript.SetActivity(UnitControlScript.ActivityType.Fighting);
                break;
            case InteractableType.Hauling:
                playerControlScript.SetActivity(UnitControlScript.ActivityType.Hauling);
                break;
        }


    }

    // Method to handle loss of focus on the object
    public virtual void OnDeFocused(Transform playerTransform)
    {
        // Remove the player from the list of interacting players
        playerList.Remove(playerTransform);
        // If there are no more interacting players, reset interaction status and focus
        if (playerList.Count == 0)
        {
            hasInteracted = false;
            isFocus = false;
        }
    }

    public enum InteractableType
    {
        None,
        Hauling,
        Enemy,
        Tree,
        Hill,
        Farm
    }
}
