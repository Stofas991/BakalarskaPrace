using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

//tøída dìdí ClickableObject která mìní cursor
public class EnemyMove : ClickableObject
{
    private NavMeshAgent agent;

    private int distance;
    private GameObject alertedUnit;
    private GameObject chasedObject;
    public Animator animator;
    public EnemyAttacked enemyAttack;

    private float attackRange;
    private float nextAttackEvent;
    public float attackDelay = 2f;
    public int attackDamage = 20;

    public LayerMask enemyLayers;
    private bool chasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        distance = 10;
        attackRange = 1.5f;
        alertedUnit = transform.Find("ExclamationRed").gameObject;
        enemyAttack = GetComponent<EnemyAttacked>();
    }

    void Update()
    {
        if (chasedObject == null)
        {
            chasing = false;
            CancelAttack();
            alertedUnit.SetActive(false);
        }

        if (!chasing)
        {
            Collider2D[] HitEnemies = Physics2D.OverlapCircleAll(agent.transform.position, distance, enemyLayers);
            if (HitEnemies.Length > 0)
            {
                chasedObject = HitEnemies[0].gameObject;
                chasing = true;
            }
            else
            {
                return;
            }
        }

        //in range of enemy, attack
        if (Vector3.Distance(agent.transform.position, chasedObject.transform.position) <= attackRange)
        {
            //stop animation
            CancelAttack();

            agent.SetDestination(agent.transform.position);
            //attack cooldown
            if (Time.time >= nextAttackEvent)
            {
                nextAttackEvent = Time.time + attackDelay;
               
                Attack();
            }
            else
            {
                CancelAttack();
            }
        }
        //out of chasing range
        else if (Vector3.Distance(agent.transform.position, chasedObject.transform.position) < distance)
        {
            //creating empty path
            NavMeshPath navMeshPath = new NavMeshPath();

            if (agent.CalculatePath(chasedObject.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                agent.ResetPath();
                agent.SetDestination(chasedObject.transform.position);
            }
            else
            {
                var collider = GetComponent<BoxCollider2D>();
                collider.enabled = false;
                RaycastHit2D hit = Physics2D.Linecast(transform.position, chasedObject.transform.position);
                collider.enabled = true;

                if (hit.collider != null && hit.collider.gameObject.GetComponent<Destroyable>())
                {
                    chasedObject = hit.collider.gameObject;
                }
                else
                {
                    agent.SetDestination(chasedObject.transform.position);
                }
            }

            //alert exclamation mark
            alertedUnit.SetActive(true);

            bool flipped = agent.transform.position.x < chasedObject.transform.position.x;
            transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }
        else
        {
            if (chasedObject.GetComponent<Destroyable>() != null)
                chasedObject.GetComponent<Destroyable>().OnDeFocused(transform);

            agent.SetDestination(agent.transform.position);
            alertedUnit.SetActive(false);
            chasedObject = null;
            chasing = false;
        }
    }

    void Attack()
    {
        //attack animation
        animator.SetTrigger("InRange");
        var destroyableScript = chasedObject.GetComponent<Destroyable>();

        //deal damage
        if (chasedObject.GetComponent<UnitStats>() != null)
            chasedObject.GetComponent<UnitStats>().TakeDamage(attackDamage);

        else if (destroyableScript != null)
        {
            chasedObject.GetComponent<Destroyable>().TakeDamage(attackDamage);
            destroyableScript.OnFocused(transform);
        }

        else
            Debug.Log("Object does not contain component that can be attacked");
    }

    void CancelAttack()
    {
        animator.ResetTrigger("InRange");
    }
}
