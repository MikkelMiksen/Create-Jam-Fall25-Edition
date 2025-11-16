using UnityEngine;

public class CatChase : MonoBehaviour
{
    public float chaseSpeed = 5f;
    private CatMovement movement;
    private CatStateMachine stateMachine;
    private CatSight sight;

    void Awake()
    {
        movement = GetComponent<CatMovement>();
        stateMachine = GetComponent<CatStateMachine>();
        sight = GetComponent<CatSight>();
    }

    public void ChasePlayer()
    {
        if (sight.player)
        {
            movement.MoveTo(sight.player.position, chaseSpeed);

            // Lose sight logic
            Vector3 lastPos;
            if (!sight.CanSeePlayer(out lastPos))
            {
                stateMachine.lastKnownPlayerPos = lastPos;
                stateMachine.SwitchState(CatStateMachine.State.Investigate);
            }
        }
    }
}
