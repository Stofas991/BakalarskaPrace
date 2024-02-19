using UnityEngine;

public class itemPickup : Interactable
{
    public Item item;
    public override void Interact(Transform interactingPlayer)
    {
        base.Interact(interactingPlayer);

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
