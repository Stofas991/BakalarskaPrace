using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockpileScript : Interactable
{
    public bool containsItem = false;
    public ContainedItemType itemType;
    public int itemCount;
    public int itemMaxCount = 100;

    [SerializeField] StockpileItemBase stockpileItem;
    ResourcesScript resourceMenu;

    private void Start()
    {
        resourceMenu = ResourcesScript.GetInstance();
    }

    public override void Interact(Transform interactingPlayer)
    {
        base.Interact(interactingPlayer);
        //if does not contain item, spawn new one
        if (!containsItem)
        {
            GameObject gameObject = stockpileItem.ItemImage;
            Instantiate(gameObject, transform);

            itemType = stockpileItem.ItemType;
            containsItem = true;

        }
        UnitControlScript control = interactingPlayer.GetComponent<UnitControlScript>();
        itemCount = control.UCCount;
        control.UCCount = 0;
        control.UCItemType = ContainedItemType.None;
        resourceMenu.UpdateAmmount(itemCount, itemType);
        interactingPlayer.GetComponent<UnitControlScript>().DestroyCarriedItem();

        hasInteracted = true;
    }
}
public enum ContainedItemType
{
    None,
    Wood,
    Stone,
    Iron
}

