using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;


public class UnitControlScript: MonoBehaviour
{
    public Vector3 target;
    private NavMeshAgent agent;
    public Interactable enemy;
    public Animator animator;

    public float attackRange;
    private float nextAttackEvent;
    public float attackDelay = 5f;
    public int attackDamage = 25;
    public Guid guid;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = agent.transform.position;
        guid = Guid.NewGuid();
    }

    // Update is called once per frame
    void Update()
    {
        CancelAttack();

        if (enemy != null)
            target = enemy.transform.position;
        
        if (enemy != null && Vector3.Distance(agent.transform.position, enemy.transform.position) <= attackRange)
        {
            agent.SetDestination(agent.transform.position);
            //pokud už mùžu útoèit, tak zaùtoèí
            if (Time.time >= nextAttackEvent)
            {
                nextAttackEvent = Time.time + attackDelay;

                Attack();
            }
        }
        else
        {

            bool flipped = target.x > agent.transform.position.x;
            if (target.z != 0)
            {
                target.z = 0;
            }

            transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
            agent.SetDestination(target);
        }
    }

    void Attack()
    {
        //zaène útoèící animaci
        animator.SetTrigger("inRange");

        //udìlí poškození nepøíteli
        enemy.myHealth.TakeDamage(attackDamage);

    }

    void CancelAttack()
    {
        animator.ResetTrigger("inRange");
    }
}
