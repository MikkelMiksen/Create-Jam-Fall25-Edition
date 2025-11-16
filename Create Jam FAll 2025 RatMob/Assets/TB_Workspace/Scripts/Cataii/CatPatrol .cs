using UnityEngine;

public class CatPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;

    private int currentIndex = 0;
    private CatMovement movement;

    void Awake()
    {
        movement = GetComponent<CatMovement>();
        if (patrolPoints.Length > 0)
            movement.MoveTo(patrolPoints[currentIndex].position, patrolSpeed);
    }

    public void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (movement.ReachedDestination())
        {
            currentIndex = (currentIndex + 1) % patrolPoints.Length;
            movement.MoveTo(patrolPoints[currentIndex].position, patrolSpeed);
        }
    }
}
