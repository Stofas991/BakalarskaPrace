using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

//t��da d�d� ClickableObject kter� m�n� cursor
public class EnemyMove : ClickableObject
{
    private NavMeshAgent agent;

    private int distance;
    private GameObject AlertedUnit;
    private GameObject ChasedEnemy;
    public Animator Animator;
    public EnemyAttacked enemyAttack;

    private float AttackRange;
    private float NextAttackEvent;
    public float AttackDelay = 5f;
    public int AttackDamage = 20;

    public LayerMask EnemyLayers;
    private bool Chasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        distance = 10;
        AttackRange = 1.5f;
        AlertedUnit = transform.Find("ExclamationRed").gameObject;
        enemyAttack = GetComponent<EnemyAttacked>();
    }

    void Update()
    {
        if (ChasedEnemy == null)
        {
            Chasing = false;
            CancelAttack();
            AlertedUnit.SetActive(false);
        }

        if (!Chasing)
        {
            Collider2D[] HitEnemies = Physics2D.OverlapCircleAll(agent.transform.position, distance, EnemyLayers);
            if (HitEnemies.Length > 0)
            {
                ChasedEnemy = HitEnemies[0].gameObject;
                enemyAttack.unitController = ChasedEnemy.GetComponent<UnitControlScript>();
                Chasing = true;
            }
            else
            {
                return;
            }
        }

        //v dosahu nep��tele, �tok
        if (Vector3.Distance(agent.transform.position, ChasedEnemy.transform.position) <= AttackRange)
        {
            //zastavit animaci
            CancelAttack();

            agent.SetDestination(agent.transform.position);
            //pokud u� m��u �to�it, tak za�to��
            if (Time.time >= NextAttackEvent)
            {
                NextAttackEvent = Time.time + AttackDelay;
               
                Attack();
            }
            else
            {
                CancelAttack();
            }
        }
        //Nen� v dosahu, p�ibl�� se na dosah
        else if (Vector3.Distance(agent.transform.position, ChasedEnemy.transform.position) < distance)
        {
            agent.SetDestination(ChasedEnemy.transform.position);
         
            //Zobraz� vyk�i�n�k nad postavou
            AlertedUnit.SetActive(true);
            bool flipped = agent.transform.position.x < ChasedEnemy.transform.position.x;
            transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }
        else
        {
            agent.SetDestination(agent.transform.position);
            AlertedUnit.SetActive(false);
            ChasedEnemy = null;
            Chasing = false;
        }
    }

    void Attack()
    {
        //za�ne �to��c� animaci
        Animator.SetTrigger("InRange");

        //ud�l� po�kozen� nep��teli
        ChasedEnemy.GetComponent<UnitStats>().TakeDamage(AttackDamage);
    }

    void CancelAttack()
    {
        Animator.ResetTrigger("InRange");
    }
}
