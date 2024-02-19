
using UnityEngine;
[RequireComponent (typeof(UnitStats))]
public class EnemyAttacked : Interactable
{
    UnitStats myStats;

    void Start()
    { 
        myStats = GetComponent<UnitStats>();
    }
    public override void Interact(Transform interactingPlayer)
    {
        base.Interact(interactingPlayer);
        CharacterCombat playerCombat = interactingPlayer.GetComponent<CharacterCombat>();

        if (playerCombat != null)
        {
            playerCombat.Attack(myStats);
        }
    }
}
