using UnityEngine;

[RequireComponent(typeof(CatMovement))]
[RequireComponent(typeof(CatPatrol))]
[RequireComponent(typeof(CatSight))]
[RequireComponent(typeof(CatStateMachine))]
[RequireComponent(typeof(CatInvestigate))]
[RequireComponent(typeof(CatChase))]
public class CatAIController : MonoBehaviour
{
    private CatMovement movement;
    private CatPatrol patrol;
    private CatSight sight;
    private CatStateMachine stateMachine;
    private CatInvestigate investigate;
    private CatChase chase;

    void Awake()
    {
        movement = GetComponent<CatMovement>();
        patrol = GetComponent<CatPatrol>();
        sight = GetComponent<CatSight>();
        stateMachine = GetComponent<CatStateMachine>();
        investigate = GetComponent<CatInvestigate>();
        chase = GetComponent<CatChase>();
    }

    void Update()
    {
        Vector3 lastPos;
        bool seesPlayer = sight.CanSeePlayer(out lastPos);

        switch (stateMachine.currentState)
        {
            case CatStateMachine.State.Patrol:
                patrol.Patrol();
                if (seesPlayer)
                    stateMachine.SwitchState(CatStateMachine.State.Chase);
                else if (lastPos != Vector3.zero)
                {
                    stateMachine.lastKnownPlayerPos = lastPos;
                    investigate.StartInvestigation(lastPos);
                    stateMachine.SwitchState(CatStateMachine.State.Investigate);
                }
                break;

            case CatStateMachine.State.Investigate:
                investigate.Investigate();
                if (seesPlayer)
                    stateMachine.SwitchState(CatStateMachine.State.Chase);
                break;

            case CatStateMachine.State.Chase:
                chase.ChasePlayer();
                break;
        }
    }
}
