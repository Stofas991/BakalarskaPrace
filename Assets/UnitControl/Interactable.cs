using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType { Enemy, tree }

public class Interactable : MonoBehaviour
{
    public UnitHealth myHealth { get; private set; }

    public InteractableType interactionType;

    private void Awake()
    {
        if(interactionType == InteractableType.Enemy) 
        { 
            myHealth = GetComponent<UnitHealth>();
        }
    }
}
