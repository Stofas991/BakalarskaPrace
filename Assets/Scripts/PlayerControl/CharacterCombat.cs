/**
* File: CharacterCombat.cs
* Author: Kry�tof Glos
* Date Last Modified: 1.5.2024
* Description: This script handles combat behavior for a character, including attacking and cooldown management.
*/
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

    /// <summary>
    /// Initiates an attack on the target unit.
    /// </summary>
    /// <param name="targetStats">Stats of the target unit.</param>
    public void Attack(UnitStats targetStats)
    {
        if (myStats.attackCooldown <= 0)
        {
            animator.SetTrigger("InRange");

            if (myStats.isRanged)
            {
                var bulletTransform = Instantiate(myStats.projectilePrefab, transform.position, Quaternion.identity);

                Vector3 shootDirection = (targetStats.transform.position - bulletTransform.transform.position).normalized;
                bulletTransform.Setup(shootDirection, myStats.attackDamage, tag);
            }
            else
                targetStats.TakeDamage(myStats.attackDamage);

            myStats.attackCooldown = 1f / myStats.attackSpeed;
        }
    }
}
