using UnityEngine;

public class CatStateMachine : MonoBehaviour
{
    public enum State { Patrol, Investigate, Chase }
    public State currentState = State.Patrol;

    [HideInInspector] public Vector3 lastKnownPlayerPos;

    public void SwitchState(State newState)
    {
        currentState = newState;
    }
}
    