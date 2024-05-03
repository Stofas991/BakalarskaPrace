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
