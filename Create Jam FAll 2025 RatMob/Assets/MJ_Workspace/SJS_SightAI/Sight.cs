using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{
    public float range = 5;
    public float FOV = 30;
    public float awareness = 1;
    public bool showGizmos = false;
    //debug
    public List<Transform> Targets_debug;
  
    public List<Transform> activate()
    {
        List<Transform> detected = GetDetectable(Physics.OverlapSphere(transform.position, range));
        for (int i = detected.Count - 1; i >= 0; i--)
        {
            Transform target = detected[i];
            if (!targetInFOV(target, FOV) || Obstructed(target))
            {
                detected.RemoveAt(i);
            }
        }
        Targets_debug = detected;
        return detected;
    }

    public List<Transform> GetDetectable(Collider[] targets)
    {
        List<Transform> list = new List<Transform>();
        foreach (Collider target in targets)
        {
            if (target.tag == "Player")
            list.Add(target.transform);
        }
        return list;
    }
    public bool targetInFOV(Transform target, float FOV)
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        return Vector3.Angle(direction, transform.forward) <= FOV;
    }
    public bool Obstructed(Transform target)
    {
        Vector3 direction = target.transform.position - transform.position;
        Ray ray = new Ray(this.transform.position, direction);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        return hit.transform != target;
    }
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, range);
            //2 rays that show the fov of the eyes
            Gizmos.color = Color.red;
            Vector3 right = Quaternion.Euler(0, FOV, 0) * transform.forward;
            Gizmos.DrawRay(this.transform.position, right * range);
            Vector3 left = Quaternion.Euler(0, -FOV, 0) * transform.forward;
            Gizmos.DrawRay(this.transform.position, left * range);
            if (Targets_debug.Count > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(this.transform.position, Targets_debug[0].transform.position - transform.position);
            }
        }

    }
}

