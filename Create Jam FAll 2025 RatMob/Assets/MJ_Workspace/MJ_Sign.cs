using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public enum names { Don, Swiftwhisker, CapoBricktail }
public class BossMissionGiver : MonoBehaviour, Iinteractable
{

    public List<Mission> missionChain = new();
    private int missionIndex = 0;

    private int dialogueIndex = 0;
    bool talking = false;
    bool NextTalkDon = true;

    [SerializeField] names name;

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
                    "Don: \"Bring the Family a tribute of real cheese.\"",
                    "Don: \"Return to ME when you have the goods.\""
                },
                new Dictionary<ItemType, int> { { ItemType.Cheese, 5 } },
                names.Don
            )
        );

        // Mission 2 — Diesel Preparation
        missionChain.Add(
            new Mission(
                "Fuel for the Family",
                new string[] {
                    "Don: \"Your cheese tribute was acceptable.\"",
                    "Don: \"Now the Family needs heat. Swiftwhisker handles fuel.\"",
                    "Don: \"Bring HIM 3 units of diesel.\""
                },
                new Dictionary<ItemType, int> { { ItemType.Diesel, 3 } },
                names.Swiftwhisker
            )
        );

        // Mission 3 — Scrap Metal for Rat Defenses
        missionChain.Add(
            new Mission(
                "Fortify the Borough",
                new string[] {
                    "Don: \"The roaches grow bold… troublesome.\"",
                    "Don: \"Capo Bricktail is reinforcing the northwest tunnel.\"",
                    "Don: \"Deliver 10 scrap metal directly to HIM.\""
                },
                new Dictionary<ItemType, int> { { ItemType.ScrapMetal, 10 } },
                names.CapoBricktail
            )
        );
    }

    public void Interact()
    {
        Interact(name);
    }
    
    public void Interact(names npc)
    {
        var mission = missionChain[missionIndex];

        // If talking to Don and mission not started yet → normal dialogue
        if (npc == names.Don && !mission.isCompleted)
        {
            ShowUI();
            return;
        }

        // If talking to the intended receiver
        if (npc == mission.deliverTo)
        {
            ShowUI(); // Shows mission UI
            return;
        }

        // WRONG NPC
        dialogueText.text =
            $"{npc} stares blankly.\n\n" +
            "This rat ain't expecting anything from you.";
        missionPanel.SetActive(true);
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

    public void NextDialogue(names npcTalkingTo)
    {
        Mission mission = missionChain[missionIndex];

        // -- CASE 1: Talking to WRONG rat
        if (npcTalkingTo != mission.deliverTo && npcTalkingTo != names.Don)
        {
            dialogueText.text =
                $"{npcTalkingTo}: \"Ey, kid… you got the wrong rat.\"" +
                $"\nFind the {mission.deliverTo}.";
            return;
        }

        // -- CASE 2: Dialogue still ongoing
        if (dialogueIndex < mission.dialogues.Length)
        {
            dialogueText.text = mission.dialogues[dialogueIndex];
            dialogueIndex++;
            return;
        }

        // -- CASE 3: Dialogue finished → Try to complete mission
        TryCompleteMission(npcTalkingTo);

        // If mission completed → reset dialogue index for next mission
        if (mission.isCompleted)
            dialogueIndex = 0;
    }

    private void TryCompleteMission(names npc)
    {
        Mission mission = missionChain[missionIndex];

        // Correct NPC?
        if (npc != mission.deliverTo)
        {
            dialogueText.text = $"{npc}: \"I ain’t expectin’ nothin’ from you, pal.\"";
            return;
        }

        // Check if player has items
        if (!HasAllRequiredItems(mission))
        {
            dialogueText.text = $"{npc}: \"You ain't got the goods yet. Scram.\"";
            return;
        }

        // Complete mission
        ConsumeRequiredItems(mission);
        mission.isCompleted = true;

        dialogueText.text = $"{npc}: \"Yeah. That’s what the Family needed. Good work.\"";

        // Move to next mission
        missionIndex = Mathf.Min(missionIndex + 1, missionChain.Count - 1);
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
