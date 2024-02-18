using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public Animator animator;

    UnitStats myStats;

    public bool attacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        myStats = GetComponent<UnitStats>();
    }

    void Update()
    {
        myStats.attackCooldown -= Time.deltaTime;
    }

    public void Attack(UnitStats targetStats)
    {
        if (myStats.attackCooldown <= 0)
        {
            animator.SetTrigger("inRange");
            targetStats.TakeDamage(myStats.attackDamage);
            myStats.attackCooldown = 1f / myStats.attackSpeed;
        }
    }
}
