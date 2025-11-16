using System.Collections.Generic;
using UnityEngine;

public enum names
{
    Don,
}
public class BossMissionGiver : MonoBehaviour, Iinteractable
{

    public List<Mission> missionChain = new();
    private int missionIndex = 0;

    int dialogueIndex = 0;
    bool talking = false;

    public GameObject missionPanel;
    public TMPro.TextMeshProUGUI dialogueText;

    MJ_InventoryHandler inventory;

    void Start()
    {
        inventory = MJ_InventoryHandler.Instance;

        // Hide UI
        missionPanel.SetActive(false);

        // Build the mission list
        BuildMissionChain();
    }

    void BuildMissionChain()
    {
        // Mission 1 — Cheese Collection
        missionChain.Add(
            new Mission(
                "The Cheese Tribute",
                new string[] {
                    "Don Rattoleone: \"We start simple, capisce?\"",
                    "Don: \"Bring the Family a tribute of *real cheese*. Not that plastic garbage.\"",
                    "Don: \"Return when you have the goods.\""
                },
                new Dictionary<ItemType,int> {
                    { ItemType.Cheese, 5 }
                }
            )
        );

        // Mission 2 — Diesel Preparation
        missionChain.Add(
            new Mission(
                "Fuel for the Family",
                new string[] {
                    "Don: \"Your tribute was acceptable. The Family acknowledges your effort.\"",
                    "Don: \"Now we need heat. The tunnels grow cold, and cold rats do not make good soldiers.\"",
                    "Don: \"Bring us *3 units of diesel*. Swiftwhisker will handle the processing.\""
                },
                new Dictionary<ItemType,int> {
                    { ItemType.Diesel, 3 }
                }
            )
        );

        // Mission 3 — Scrap Metal for Rat Defenses
        missionChain.Add(
            new Mission(
                "Fortify the Borough",
                new string[] {
                    "Don: \"The roaches grow bold. Their numbers… troublesome.\"",
                    "Don: \"Capo Bricktail wants to reinforce the northwest tunnel.\"",
                    "Don: \"Fetch *10 scrap metal* so we can build proper barricades.\""
                },
                new Dictionary<ItemType,int> {
                    { ItemType.ScrapMetal, 10 }
                }
            )
        );
    }

    public void Interact()
    {
        ShowUI();
    }

    void ShowUI()
    {
        talking = true;
        missionPanel.SetActive(true);

        dialogueIndex = 0;

        DisplayCurrentDialogue();
    }

    public string GetPrompt() => "Press E to talk with the Boss";

    void DisplayCurrentDialogue()
    {
        var mission = missionChain[missionIndex];
        dialogueText.text = mission.dialogues[dialogueIndex];
    }

    public void NextDialogue()
    {
        if (!talking) return;

        var mission = missionChain[missionIndex];

        dialogueIndex++;

        // End of dialogs → check mission completion
        if (dialogueIndex >= mission.dialogues.Length)
        {
            TryCompleteMission();
            return;
        }

        DisplayCurrentDialogue();
    }

    void TryCompleteMission()
    {
        var mission = missionChain[missionIndex];

        if (HasAllRequiredItems(mission))
        {
            ConsumeRequiredItems(mission);
            mission.isCompleted = true;

            dialogueText.text =
                $"Don: \"Well done. The Family is pleased.\"\n\n" +
                $"Mission Completed: {mission.missionTitle}";

            missionIndex = Mathf.Min(missionIndex + 1, missionChain.Count - 1);
        }
        else
        {
            dialogueText.text =
                "Don: \"Come back when you have what I asked for. " +
                "I do not repeat myself.\"";
        }
    }

    bool HasAllRequiredItems(Mission mission)
    {
        foreach (var pair in mission.requiredItems)
        {
            if (!inventory.carriedItems.ContainsKey(pair.Key)) return false;
            if (inventory.carriedItems[pair.Key] < pair.Value) return false;
        }
        return true;
    }

    void ConsumeRequiredItems(Mission mission)
    {
        foreach (var pair in mission.requiredItems)
            inventory.carriedItems[pair.Key] -= pair.Value;
    }

    public void CloseUI()
    {
        missionPanel.SetActive(false);
        talking = false;
    }
}
