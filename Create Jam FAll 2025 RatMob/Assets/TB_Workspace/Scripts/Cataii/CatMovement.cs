using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CatMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 destination, float speed)
    {
        agent.speed = speed;
        agent.destination = destination;
    }

    public bool ReachedDestination(float threshold = 0.5f)
    {
        return !agent.pathPending && agent.remainingDistance <= threshold;
    }
}
