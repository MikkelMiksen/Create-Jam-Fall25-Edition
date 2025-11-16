using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    public float glanceDecay = 1;
    public GameObject senses;
    public List<Transform> waypointParent = new();
    //ref
    private Sight[] sights;
    private NavMeshAgent agent;
    private Transform player;
    private EnemyScript owner;
    //used vars
    private int currentWaypoint = 0;
    private float glanceMeter;
    private void Awake()
    {
        owner = GetComponent<EnemyScript>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sights = senses.GetComponents<Sight>();
    }
    public override void StateStart(Transform target = null)
    {

    }

    public override void StateUpdate()
    {
        glance();

    }
    public void glance()
    {
        Glance.glance(ref glanceMeter, ref glanceDecay, sights);
        owner.healthBar.BarUpdate(glanceMeter, Color.yellow);
        if (glanceMeter >= 1)
        {
            owner.switchState(EnemyScript.StateEnum.investigating, player);
            glanceMeter = 0;
        }
    }
}
