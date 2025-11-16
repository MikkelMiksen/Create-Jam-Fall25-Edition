using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public string missionTitle;
    public string[] dialogues;
    public Dictionary<ItemType, int> requiredItems;

    public names deliverTo;   // 🔥 NEW: Who the items must be delivered to

    public bool isCompleted = false;

    public Mission(string title, string[] dialogues, Dictionary<ItemType, int> req, names deliverTo)
    {
        missionTitle = title;
        this.dialogues = dialogues;
        requiredItems = req;
        this.deliverTo = deliverTo;
    }
}