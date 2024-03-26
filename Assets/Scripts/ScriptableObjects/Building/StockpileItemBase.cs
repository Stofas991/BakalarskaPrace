using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buildable", menuName = "Building/Create Stockpile Item")]
public class StockpileItemBase : ScriptableObject
{
    [SerializeField] GameObject itemImage;
    [SerializeField] ContainedItemType itemType;

    public GameObject ItemImage
    { 
        get { return itemImage; } 
    }

    public ContainedItemType ItemType
    { 
        get { return itemType; } 
    }
}
