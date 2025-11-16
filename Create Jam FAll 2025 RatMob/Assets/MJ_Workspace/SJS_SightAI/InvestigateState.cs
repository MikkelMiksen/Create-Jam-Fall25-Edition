using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class InvestigateState : State
{
    //ref
    private NavMeshAgent agent;
    private EnemyScript owner;
    private Transform player;
    private Sight[] sights;
    //inspector values
    [SerializeField] private float glanceDecay;
    [SerializeField] private GameObject senses;
    [SerializeField] private float investigationTime;
    [SerializeField] private float TimePerSpot;
    //used vars
    private Vector3 point;
    private float nextMovement;
    private float timeSpent;
    private float glanceMeter;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        owner = GetComponent<EnemyScript>();
        sights = senses.GetComponents<Sight>();
    }
    public override void StateStart(Transform target = null)
    {
        point = target == null ? transform.position : target.position;
        agent.speed = owner.walkSpeed;

        timeSpent = 0;
        nextMovement = 0;
    }
    public override void StateUpdate()
    {
        glance();
        if (Time.time > nextMovement)
        {

            if (timeSpent >= investigationTime) owner.switchState(EnemyScript.StateEnum.patrolling, null, true);
            nextMovement = Time.time + TimePerSpot;
            timeSpent += TimePerSpot;
            agent.SetDestination(point + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3)));
            
        }

    }
    public void glance()
    {
        
        Glance.glance(ref glanceMeter, ref glanceDecay, sights);
        owner.healthBar.BarUpdate(glanceMeter, Color.red);
        if (glanceMeter >= 1)
        {
            owner.switchState(EnemyScript.StateEnum.chasing, player);
            glanceMeter = 0;

        }


    }


}
