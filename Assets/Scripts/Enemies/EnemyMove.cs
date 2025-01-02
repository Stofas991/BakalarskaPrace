/**
 * File: EnemyMove.cs
 * Author: Kryštof Glos
 * Last Modified: 1.5.2024
 * Description: Script for controlling enemy movement, detection, and attacks.
 */

using UnityEngine;
using UnityEngine.AI;

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
    private float nextAttackEvent;
    private UnitStats stats;

    //promìnná pro detekci hráèe
    private int distance = 100;
    void Start()
    {
        // Inicializace promìnných
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        alertedUnit = transform.Find("ExclamationRed").gameObject;
        enemyAttack = GetComponent<EnemyAttacked>();
        stats = GetComponent<UnitStats>();
        agent.speed = stats.movementSpeed;
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
        chasedObject = null;
    }

    // Metoda pro detekci hráèe
    void CheckForPlayer()
    {
        Collider2D[] HitEnemies = Physics2D.OverlapCircleAll(agent.transform.position, distance, enemyLayers);
        if (HitEnemies.Length > 0)
        {
            chaseUnit(HitEnemies[0].gameObject);
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
        else if (IsObstacleInWay())
        {
            HandleObstacle();
        }
        else if (!IsInChasingRange())
        {
            StopChasing();
        }
        else
            MoveTowardsPlayer();

    }

    // Method to check if player is in attacking range
    bool IsInRangeOfEnemy()
    {
        return Vector3.Distance(agent.transform.position, chasedObject.transform.position) <= stats.attackRange;
    }

    // Method to check if player is in chasing range
    bool IsInChasingRange()
    {
        return Vector3.Distance(agent.transform.position, chasedObject.transform.position) <= distance;
    }

    // Method for movement towards target
    void MoveTowardsPlayer()
    {
        if (agent.destination != chasedObject.transform.position)
        {
            agent.SetDestination(chasedObject.transform.position);

            alertedUnit.SetActive(true);
            RotateTowardsPlayer();
        }
    }

    // Metoda pro zpracování pøekážky mezi nepøítelem a hráèem
    bool IsObstacleInWay()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        return !(agent.CalculatePath(chasedObject.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete);
    }

    void HandleObstacle()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        int buildingsLayerMask = 1 << LayerMask.NameToLayer("Buildings");
        RaycastHit2D hit = Physics2D.Linecast(transform.position, chasedObject.transform.position, buildingsLayerMask);
        collider.enabled = true;

        if (hit.collider != null && hit.collider.gameObject.GetComponent<Destroyable>() && hit.collider.gameObject != chasedObject)
        {
            chasedObject = hit.collider.gameObject;
        }
        agent.SetDestination(chasedObject.transform.position);
    }

    void Attack()
    {
        //stopping agent at max range
        agent.SetDestination(agent.transform.position);
        //Checking if attack cooldown is off
        if (Time.time >= nextAttackEvent)
        {
            nextAttackEvent = Time.time + stats.attackSpeed;

            animator.SetTrigger("InRange");
            var target = chasedObject.GetComponent<IAttackable>();

            if (target != null)
            {
                if (stats.isRanged)
                {
                    var bulletTransform = Instantiate(stats.projectilePrefab, transform.position, Quaternion.identity);

                    Vector3 shootDirection = (chasedObject.transform.position - bulletTransform.transform.position).normalized;
                    bulletTransform.Setup(shootDirection, stats.attackDamage, tag);
                }
                else
                    target.TakeDamage(stats.attackDamage);
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

    public void chaseUnit(GameObject unit)
    {
        chasedObject = unit;
        chasing = true;
    }
}
