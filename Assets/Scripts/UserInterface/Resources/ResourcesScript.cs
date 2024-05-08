/*
 * File: ResourcesScript.cs
 * Description: Manages game resources and their display in the UI.
 * Author: Kryštof Glos
 * Date: 1.5.2024
 */
using TMPro;
using UnityEngine;

public class ResourcesScript : Singleton<ResourcesScript>
{
    [SerializeField] public Resource[] resourceList;

    ///<summary>
    /// Updates the amount of a specific resource and updates its display in the UI.
    ///</summary>
    ///<param name="amount">The amount to update the resource by.</param>
    ///<param name="type">The type of resource to update.</param>
    public void UpdateAmmount(int ammount, ContainedItemType type)
    {
        foreach (var resource in resourceList)
        {
            if (resource.requiredResources.itemType == type)
            {
                resource.requiredResources.ammount += ammount;
                resource.text.text = resource.requiredResources.ammount.ToString();
                break;
            }
        }
    }
}

[System.Serializable]
public class Resource
{
    public TMP_Text text;
    public RequiredResources requiredResources;
}
