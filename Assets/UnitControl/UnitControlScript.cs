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
    [SerializeField] GameObject clickEffect;

    public Vector3 target;
    private NavMeshAgent agent;
    public Interactable enemy;
    public Animator animator;
    public Interactable focus;
    private PlayerMotor motor;
    
    public float attackRange;
    public float nextAttackEvent;
    public float attackDelay = 5f;
    public Guid guid;
    public bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        target = agent.transform.position;
        guid = Guid.NewGuid();
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            CancelAttack();
            /*
            if (enemy != null)
                target = enemy.transform.position;

            if (enemy != null && Vector3.Distance(agent.transform.position, enemy.transform.position) <= attackRange)
            {
                motor.MoveToPoint(agent.transform.position);
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
                motor.MoveToPoint(target);
            }
            */
            if (Input.GetMouseButtonDown(1))
            {
                //kontrola jestli se kliknulo na nepøítele
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit)
                {
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    if (interactable != null)
                    {
                        SetFocus(interactable);
                    }
                }
                else
                {
                    RemoveFocus();
                    target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    motor.MoveToPoint(target);

                    //particle effekt pro zobrazení bodu k pohybu
                    if (clickEffect != null)
                    {
                        Vector3 positionZeroedZ = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        positionZeroedZ.z = 0;
                        Instantiate(clickEffect, positionZeroedZ, clickEffect.transform.rotation);
                    }
                }
            }
        }
    }

    void CancelAttack()
    {
        animator.ResetTrigger("inRange");
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null)
                focus.OnDeFocused();

            focus = newFocus;
            newFocus.OnFocused(transform);
            motor.FollowTarget(newFocus, attackRange);
        }
    }

    void RemoveFocus()
    {
        if (focus != null)
        {
            if (focus.tag == "Tree")
            {
                focus.transform.GetChild(0).gameObject.SetActive(false);
            }
            focus.OnDeFocused();
        }



        focus = null;
        motor.StopFollowingTarget();
    }

}
