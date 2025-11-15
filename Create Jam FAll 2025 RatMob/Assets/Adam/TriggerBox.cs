using UnityEngine;

public class TriggerBox : MonoBehaviour
{
    public TriggerType triggerType;
    public int sceneIndexToLoad;
}

public enum TriggerType
{
    LevelChangeTrigger, Interact
}

