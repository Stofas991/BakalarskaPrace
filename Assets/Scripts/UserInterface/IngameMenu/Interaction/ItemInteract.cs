using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

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
        GameObject player = playerList[0].gameObject;
        UnitControlScript unitControlScript = player.GetComponent<UnitControlScript>();

        //destroy menu
        Destroy(instance);
        if (unitControlScript.StockpileGetter() == null)
        {
            Debug.Log("There is no stockpile zone, first create one in Zones -> Stockpile zone");
            return;
        }



        //copy values for effectivity and spawn canvas
        ItemSpecifics itemSpecifics = GetComponent<ItemSpecifics>();
        unitControlScript.PickUp(itemCanvas, itemSpecifics.count, itemSpecifics.itemType);

        //Destroy this object
        Destroy(gameObject);
    }
}