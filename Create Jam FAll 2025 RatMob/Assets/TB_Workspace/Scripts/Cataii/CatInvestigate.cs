using UnityEngine;

public class CatInvestigate : MonoBehaviour
{
    public float investigateTime = 3f;
    public float investigateSpeed = 3f;

    private float timer;
    private CatMovement movement;
    private CatStateMachine stateMachine;

    void Awake()
    {
        movement = GetComponent<CatMovement>();
        stateMachine = GetComponent<CatStateMachine>();
    }

    public void StartInvestigation(Vector3 position)
    {
        movement.MoveTo(position, investigateSpeed);
        timer = investigateTime;
    }

    public void Investigate()
    {
        if (movement.ReachedDestination())
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                stateMachine.SwitchState(CatStateMachine.State.Patrol);
            }
        }
    }
}
