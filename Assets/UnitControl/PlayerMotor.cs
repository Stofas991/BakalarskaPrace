using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    Transform target;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target != null)
        {
            target.position = FlipAgent(target.position);
            agent.SetDestination(target.position);
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        point = FlipAgent(point);
        agent.SetDestination(point);
    }

    public void FollowTarget(Interactable newTarget)
    {
        target = newTarget.transform;
        if (newTarget.CompareTag("Enemy"))
        {
            newTarget.isEnemy = true;
        }
    }

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
