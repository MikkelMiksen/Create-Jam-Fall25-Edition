using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : State
{
    //ref
    private NavMeshAgent agent;
    private EnemyScript owner;
    private Transform player;
    
    private void Awake()
    {
        owner = GetComponent<EnemyScript>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public override void StateStart(Transform target = null)
    {
        agent.speed = owner.runSpeed;
    }

    public override void StateUpdate()
    {
        Vector3 direction = player.transform.position - transform.position;

        Ray ray = new Ray(this.transform.position, direction);

        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.collider!=null && hit.collider.gameObject.transform == player)
        {
            agent.SetDestination(player.position);
        } 
        else if (Vector3.Distance(transform.position,agent.destination)<agent.stoppingDistance)
        {
            owner.switchState(EnemyScript.StateEnum.investigating, transform, true);
        }
    }
}
