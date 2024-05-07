/**
 * File: CameraController.cs
 * Author: Kryštof Glos
 * Date: 3.5.2024
 * Description: This script handles the behaviour of stockpiles.
 */

using UnityEngine;

public class StockpileScript : Interactable
{
    public bool containsItem = false;
    public ContainedItemType itemType;
    public int itemCount;
    public int itemMaxCount = 100;

    [SerializeField] StockpileItemBase woodItembase;
    [SerializeField] StockpileItemBase stoneItemBase;
    ResourcesScript resourceMenu;

    private void Start()
    {
        resourceMenu = ResourcesScript.GetInstance();
    }

    public override void Interact(Transform interactingPlayer)
    {
        base.Interact(interactingPlayer);

        UnitControlScript control = interactingPlayer.GetComponent<UnitControlScript>();
        //if does not contain item, spawn new one
        if (!containsItem)
        {
            GameObject gameObject;
            switch (control.UCItemType)
            {
                case ContainedItemType.Wood:
                    gameObject = woodItembase.ItemImage;
                    itemType = woodItembase.ItemType;
                    break;
                case ContainedItemType.Stone:
                    gameObject = stoneItemBase.ItemImage;
                    itemType = stoneItemBase.ItemType;
                    break;
                default:
                    gameObject = null;
                    break;
            }
            if (gameObject != null)
            {
                Instantiate(gameObject, transform);
                containsItem = true;
            }
        }

        itemCount = control.UCCount;
        control.UCCount = 0;
        control.UCItemType = ContainedItemType.None;
        resourceMenu.UpdateAmmount(itemCount, itemType);
        interactingPlayer.GetComponent<UnitControlScript>().DestroyCarriedItem();

        hasInteracted = true;
        OnDeFocused(interactingPlayer);
    }
}

public enum ContainedItemType
{
    None,
    Wood,
    Stone,
    Iron
}

