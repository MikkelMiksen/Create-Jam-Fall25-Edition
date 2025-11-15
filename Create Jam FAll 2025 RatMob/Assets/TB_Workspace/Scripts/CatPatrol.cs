using UnityEngine;

public class CatPatrol : MonoBehaviour
{
    public Transform patrolParent;
    private Transform[] patrolPoints;

    public float speed = 2f;
    public float rotationSpeed = 5f;

    private int currentPoint = 0;
    private Animator animator;

    private Vector3 lastPosition;

    void Start()
    {
        // Setup animator
        animator = GetComponent<Animator>();

        // Save starting position for speed calculations
        lastPosition = transform.position;

        // Collect all child transforms except the parent itself
        patrolPoints = patrolParent.GetComponentsInChildren<Transform>();
        patrolPoints = System.Array.FindAll(patrolPoints, p => p != patrolParent);
    }

    void Update()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPoint];

        // --- ROTATION ---
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // prevent tilting up/down

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }

        // --- MOVEMENT ---
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }

        // --- ANIMATOR SPEED FLOAT ---
        float actualSpeed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed);

        lastPosition = transform.position;
    }
}
