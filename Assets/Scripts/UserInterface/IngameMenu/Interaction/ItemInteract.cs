using TMPro;
using UnityEngine;

public class ItemInteract : Interactable
{
    [SerializeField] GameObject interactionMenu;
    [SerializeField] GameObject itemCanvas;
    GameObject instance;

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

    public override void OnDeFocused(Transform playerTransform)
    {
        base.OnDeFocused(playerTransform);
        if (instance != null)
        {
            instance.SetActive(false);
        }
    }

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