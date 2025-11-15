using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class PatrolUnit : Entity
{
    public static PatrolUnit Instance;


    [SerializeField] private EntityTypes entityType = EntityTypes.Cat;

    public List<Transform> route = new List<Transform>();

    public bool isPatrolManagerReady = false;
    public bool routeReady => route.Count > 0;

    private int currentIndex = 0;
    private int direction = 1; // 1 = forward, -1 = backward

    // Animation stuff
    private Animator animator;
    private Vector3 lastPosition;

    void Awake()
    {
        Instance = this;
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent =GetComponentInParent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (isPatrolManagerReady)
        {
            PatrolRouteManager.Instance.GetYourPatrolRoute(entityType);
            isPatrolManagerReady = false;

            if (routeReady)
            {
                currentIndex = 0;
                SetNextDestination();
            }
        }


        if (!routeReady || agent == null || agent.pathPending)
            return;

        // Check if agent reached current destination
        if (agent.remainingDistance <= agent.stoppingDistance && routeReady)
        {
            HandleNextWaypoint();
        }

        Debug.Log("PatrolUnit" + routeReady + " : " + agent.remainingDistance);

        HandleAnimation();
    }

    void HandleNextWaypoint()
    {
        // If reached the last waypoint (going forward)
        if (currentIndex == route.Count - 1 && direction == 1)
        {
            // 50% chance: loop to start or reverse direction
            if (Random.value < 0.5f)
            {
                currentIndex = 0;
                direction = 1;
            }
            else
            {
                direction = -1;
                currentIndex--;
            }
        }
        // If reached the first waypoint (going backward)
        else if (currentIndex == 0 && direction == -1)
        {
            // 50% chance: loop to end or go forward again
            if (Random.value < 0.5f)
            {
                currentIndex = route.Count - 1;
                direction = -1;
            }
            else
            {
                direction = 1;
                currentIndex++;
            }
        }
        else
        {
            currentIndex += direction;
        }

        SetNextDestination();
    }

    void SetNextDestination()
    {
        if (route.Count == 0) return;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(route[currentIndex].position, out hit, 5.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        } return;
    }

    void HandleAnimation()
    {
        float actualSpeed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed);

        lastPosition = transform.position;
    }
}