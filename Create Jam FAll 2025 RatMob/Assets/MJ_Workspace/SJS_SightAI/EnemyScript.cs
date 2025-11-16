using UnityEngine;
public class EnemyScript : MonoBehaviour
{
    //ref

   [HideInInspector] public EnemyHealthbar healthBar;
    private PatrolState patrol;
    private InvestigateState investi;
    private ChaseState chase;
    private StateEnum state = StateEnum.inactive;
    //stats
    public float walkSpeed = 2.5f;
    public float runSpeed = 5;
    private void Awake()
    {
        patrol = GetComponent<PatrolState>();
        investi = GetComponent<InvestigateState>();
        chase = GetComponent<ChaseState>();
    }
    private void Start()
    {
        healthBar = GetComponentInChildren<EnemyHealthbar>();
        switchState(StateEnum.patrolling, null, true);

    }
    private void Update()
    {
        switch (state)
        {
            case StateEnum.patrolling:
                patrol.StateUpdate();
                
                break;
            case StateEnum.investigating:
                investi.StateUpdate();
                break;
            case StateEnum.chasing:
                chase.StateUpdate();
                break;
            default:
                break;
        }
    }

    public void switchState(StateEnum newState, Transform target = null, bool priority = false)
    {
        switch (newState)
        {
            case StateEnum.patrolling:
                if (priority)
                {
                    state = StateEnum.patrolling;
                    patrol.StateStart(target);

                }
                break;
            case StateEnum.investigating:
                if (priority || state != StateEnum.chasing)
                {
                    state = StateEnum.investigating;
                    investi.StateStart(target);
                }
                break;
            case StateEnum.chasing:
                state = StateEnum.chasing;
                chase.StateStart(target);
                break;
            default:
                break;
        }
    }
 
    public enum StateEnum
    {
        patrolling, investigating, chasing,inactive
    }

}
