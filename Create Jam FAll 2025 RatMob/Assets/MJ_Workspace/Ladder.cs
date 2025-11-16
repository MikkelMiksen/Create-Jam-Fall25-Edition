using UnityEngine;

public class Ladder : MonoBehaviour, Iinteractable
{
    [SerializeField]
    private Transform top, bottom;

    public enum GoTo
    {
        top,
        bottom,
    }
    [SerializeField]
    private GoTo destination;

    public void Interact()
    {
        GameObject tmp = GameObject.FindGameObjectWithTag("Player");

        if (destination == GoTo.top)
        {
            tmp.transform.position = top.transform.position;
        }
        else
        {
            tmp.transform.position = bottom.transform.position;
        }

    }

    public string GetPrompt() => "Press E to climb the Ladder";
}
