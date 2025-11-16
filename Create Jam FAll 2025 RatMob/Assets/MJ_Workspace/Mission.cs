using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public string missionTitle;
    public string[] dialogues;   // multiple lines for progression
    public Dictionary<ItemType, int> requiredItems;
    public bool isCompleted = false;

    public Mission(string title, string[] dialogues, Dictionary<ItemType, int> req)
    {
        missionTitle = title;
        this.dialogues = dialogues;
        requiredItems = req;
    }
}