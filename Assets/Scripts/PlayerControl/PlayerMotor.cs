/**
* File: PlayerMotor.cs
* Author: Kryštof Glos
* Date Last Modified: 18.2.2024
* Description: This script controls the movement of the player character using NavMeshAgent.
*/
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = GetComponent<UnitStats>().movementSpeed;
    }

    void Update()
    {
        if (target != null)
        {
            target.position = FlipAgent(target.position);
            agent.SetDestination(target.position);
        }
    }

    /// <summary>
    /// Moves the player character to the specified point on the NavMesh.
    /// </summary>
    /// <param name="point">The point to move to.</param>
    public void MoveToPoint(Vector3 point)
    {
        point = FlipAgent(point);
        agent.SetDestination(point);
    }

    /// <summary>
    /// Sets the player character to follow a target.
    /// </summary>
    /// <param name="newTarget">The new target to follow.</param>
    /// <param name="attackRange">The attack range when following an enemy.</param>
    public void FollowTarget(Interactable newTarget, float attackRange)
    {
        target = newTarget.transform;
        if (newTarget.CompareTag("Enemy"))
        {
            newTarget.isEnemy = true;
            agent.stoppingDistance = attackRange;
        }
        else
        {
            agent.stoppingDistance = 1f;
        }


    }

    /// <summary>
    /// Stops the player character from following the target.
    /// </summary>
    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0f;
        target = null;
    }

    private Vector3 FlipAgent(Vector3 point)
    {
        bool flipped = point.x > agent.transform.position.x;
        if (point.z != 0)
        {
            point.z = 0;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));

        return point;
    }

}
