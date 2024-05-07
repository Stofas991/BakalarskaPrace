/*
 * File: ItemInteract.cs
 * Description: Manages the interaction with items in the game world.
 * Author: Kryštof Glos
 * Date: 6.5.2024
 */
using TMPro;
using UnityEngine;

public class ItemInteract : Interactable
{
    [SerializeField] GameObject interactionMenu;
    [SerializeField] GameObject itemCanvas;
    GameObject instance;

    ///<summary>
    /// Overrides the Interact method from the base class Interactable. Handles the interaction with the item.
    ///</summary>
    ///<param name="interactingPlayer">The transform of the player interacting with the item.</param>
    public override void Interact(Transform interactingPlayer)
    {
        base.Interact(interactingPlayer);
        if (instance == null)
        {
            instance = Instantiate(interactionMenu, transform);
            GameObject child = instance.transform.GetChild(0).gameObject;
            TMP_Text[] text = child.GetComponentsInChildren<TMP_Text>();
            foreach (var t in text)
            {
                if (t.name == "ItemNameText")
                    t.text = name;

                else
                    t.text = "Haul";
            }
        }
        else
        {
            instance.SetActive(true);
        }
        hasInteracted = true;
    }

    ///<summary>
    /// Overrides the OnDeFocused method from the base class Interactable. Handles the behavior when the player is no longer interacting with the item.
    ///</summary>
    ///<param name="playerTransform">The transform of the player.</param>
    public override void OnDeFocused(Transform playerTransform)
    {
        base.OnDeFocused(playerTransform);
        if (instance != null)
        {
            instance.SetActive(false);
        }
    }

    ///<summary>
    /// Destroys the interaction menu and allows the player to carry the item.
    ///</summary>
    public void DestroyInstanceAndCarry()
    {
        ItemSpecifics itemSpecifics = GetComponent<ItemSpecifics>();
        GameObject player = GetFirstPossibleUnit(itemSpecifics.itemType);
        if (player == null)
        {
            Debug.Log("No player has open slot.");
            return;
        }
        UnitControlScript unitControlScript = player.GetComponent<UnitControlScript>();

        //destroy menu
        Destroy(instance);
        if (unitControlScript.StockpileGetter() == null)
        {
            Debug.Log("There is no stockpile zone, first create one in Zones -> Stockpile zone");
            return;
        }

        //copy values for effectivity and spawn canvas
        var pickedUp = unitControlScript.PickUp(itemCanvas, itemSpecifics.count, itemSpecifics.itemType);

        //Destroy this object only if item was picked up
        if (pickedUp)
            Destroy(gameObject);
    }

    ///<summary>
    /// Gets the first possible unit that can carry the specified item type.
    ///</summary>
    ///<param name="itemType">The type of item to be carried.</param>
    ///<returns>The GameObject of the unit, if found; otherwise, returns null.</returns>
    public GameObject GetFirstPossibleUnit(ContainedItemType itemType)
    {
        GameObject returnedUnit = null;
        foreach (var unit in playerList)
        {
            var unitController = unit.GetComponent<UnitControlScript>();
            if (unitController.UCItemType == ContainedItemType.None || unitController.UCItemType == itemType)
            {
                returnedUnit = unit.gameObject;
                return returnedUnit;
            }
        }
        return returnedUnit;
    }
}