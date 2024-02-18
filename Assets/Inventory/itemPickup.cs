using UnityEngine;

public class itemPickup : Interactable
{
    public Item item;
    public override void Interact()
    {
        base.Interact();

        PickUp();
    }

    void PickUp()
    {
        Debug.Log("picking up " + item.name);

        bool wasPicked = Inventory.Instance.Add(item);
        
        if (wasPicked)
            Destroy(gameObject);

    }
}
