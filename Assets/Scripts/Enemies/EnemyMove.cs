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

    // Promìnné pro pohyb a detekci hráèe
    private GameObject chasedObject;
    private bool chasing = false;
    public LayerMask enemyLayers;

    // Promìnné pro útok
    private GameObject alertedUnit;
    public Animator animator;
    public EnemyAttacked enemyAttack;
    public float attackRange = 1.5f;
    private float nextAttackEvent;
    public float attackDelay = 2f;
    public int attackDamage = 20;

    //promìnná pro detekci hráèe
    private int distance = 10;
    void Start()
    {
        // Inicializace promìnných
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        alertedUnit = transform.Find("ExclamationRed").gameObject;
        enemyAttack = GetComponent<EnemyAttacked>();
    }

    void Update()
    {
        if (chasedObject == null)
            StopChasing();

        if (!chasing)
            CheckForPlayer();

        if (chasing)
            HandleChase();
        else
            StopChasing();
    }

    // Metoda pro zastavení pronásledování
    void StopChasing()
    {
        chasing = false;
        CancelAttack();
        alertedUnit.SetActive(false);
    }

    // Metoda pro detekci hráèe
    void CheckForPlayer()
    {
        Collider2D[] HitEnemies = Physics2D.OverlapCircleAll(agent.transform.position, distance, enemyLayers);
        if (HitEnemies.Length > 0)
        {
            chasedObject = HitEnemies[0].gameObject;
            chasing = true;
        }
    }

    // Method handling player chase
    void HandleChase()
    {
        if (IsInRangeOfEnemy())
        {
            CancelAttack();
            Attack();
        }
        else if (IsInChasingRange())
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopChasing();
        }
    }

    // Method to check if player is in range
    bool IsInRangeOfEnemy()
    {
        return Vector3.Distance(agent.transform.position, chasedObject.transform.position) <= attackRange;
    }

    // Method to check if player is in attacking range
    bool IsInChasingRange()
    {
        return Vector3.Distance(agent.transform.position, chasedObject.transform.position) < distance;
    }

    // Metoda pro pohyb smìrem k hráèi
    void MoveTowardsPlayer()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (agent.CalculatePath(chasedObject.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            agent.ResetPath();
            agent.SetDestination(chasedObject.transform.position);
        }
        else
        {
            HandleObstacle();
        }

        alertedUnit.SetActive(true);
        RotateTowardsPlayer();
    }

    // Metoda pro zpracování pøekážky mezi nepøítelem a hráèem
    void HandleObstacle()
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

    void Attack()
    {
        //stopping agent at max range
        agent.SetDestination(agent.transform.position);
        //Checking if attack cooldown is off
        if (Time.time >= nextAttackEvent)
        {
            nextAttackEvent = Time.time + attackDelay;

            animator.SetTrigger("InRange");

            var destroyableScript = chasedObject.GetComponent<Destroyable>();
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
    }

    void CancelAttack()
    {
        animator.ResetTrigger("InRange");
    }

    // Rotation method
    void RotateTowardsPlayer()
    {
        bool flipped = agent.transform.position.x < chasedObject.transform.position.x;
        transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
    }
}
