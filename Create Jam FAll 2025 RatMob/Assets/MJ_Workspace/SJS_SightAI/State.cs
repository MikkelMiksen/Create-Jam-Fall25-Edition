using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract void StateStart(Transform target = null);
    public abstract void StateUpdate();
}
