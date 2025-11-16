using UnityEngine;

public class CatSight : MonoBehaviour
{
    public float sightRange = 10f;
    public float sightAngle = 45f;
    public Transform player;

    void Awake()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public bool CanSeePlayer(out Vector3 lastKnownPosition)
    {
        lastKnownPosition = Vector3.zero;
        if (!player) return false;

        Vector3 dir = player.position - transform.position;
        float angle = Vector3.Angle(dir, transform.forward);

        if (dir.magnitude < sightRange && angle < sightAngle)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir.normalized, out hit, sightRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    lastKnownPosition = player.position;
                    return true;
                }
                else
                {
                    lastKnownPosition = player.position;
                }
            }
        }

        return false;
    }
}
