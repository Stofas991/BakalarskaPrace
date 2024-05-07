/**
* File: StockpileItemBase.cs
* Author: Kryštof Glos
* Date Last Modified: 26.3.2024
* Description: This script defines properties for items that can be stored in a stockpile.
*/
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
