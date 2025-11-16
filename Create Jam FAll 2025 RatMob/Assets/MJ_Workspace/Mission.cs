using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public string title;
    public string[] dialogues; // used for Don's introduction
    public Dictionary<ItemType, int> requiredItems;
    public names deliverTo;

    // State flags
    public bool started = false;        // set when Don first gives the mission (intro begun)
    public bool introComplete = false;  // set when Don's introduction dialogue is finished
    public bool isCompleted = false;    // set when required items are delivered
    public Mission(string title, string[] dialogues, Dictionary<ItemType,int> req, names deliverTo)
    {
        this.title = title;
        this.dialogues = dialogues;
        this.requiredItems = req;
        this.deliverTo = deliverTo;
    }
}