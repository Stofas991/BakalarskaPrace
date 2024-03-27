using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ResourcesScript : Singleton<ResourcesScript>
{
    [SerializeField] public Resource[] resourceList;

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
