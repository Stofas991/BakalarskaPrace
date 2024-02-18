
using UnityEngine;
[RequireComponent (typeof(UnitStats))]
public class EnemyAttacked : Interactable
{

    public UnitControlScript unitController;
    UnitStats myStats;

    void Start()
    { 
        myStats = GetComponent<UnitStats>();
    }
    public override void Interact()
    {
        base.Interact();
        CharacterCombat playerCombat = unitController.GetComponent<CharacterCombat>();

        if (playerCombat != null)
        {
            playerCombat.Attack(myStats);
        }
    }
}
