using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple enum you already had
public enum names { Don, Swiftwhisker, CapoBricktail }

public class BossMissionGiver : MonoBehaviour, Iinteractable
{
    public List<Mission> missionChain = new();
    public int missionIndex = 0;

    // dialogue index used for showing multi-line intros (Don)
    private int dialogueIndex = 0;

    [SerializeField] names name; // which NPC this component belongs to
    public GameObject missionPanel;
    public TMPro.TextMeshProUGUI dialogueText;

    MJ_InventoryHandler inventory;

    private Mission theone;
    // UI / runtime flags
    private bool inDialogue = false;       // true while dialogue UI is open
    public bool missionInProgress = false; // true when a mission has been started but not completed
    public bool THATONE = false;

    // store who opened the dialogue so the Next button can use it
    private names currentTalkingNPC;

    void Awake()
    {
        inventory = MJ_InventoryHandler.Instance;
        missionPanel.SetActive(false);
        BuildMissionChain();
        theone = missionChain[missionIndex];
        THATONE = theone.introComplete;
    }

    void BuildMissionChain()
    {
        missionChain.Add(new Mission(
            "The Cheese Tribute",
            new string[] {
                "Don: \"We start simple, capisce?\"",
                "Don: \"Bring the Family a tribute of real cheese.\"",
                "Don: \"Return to ME when you have the goods.\""
            },
            new Dictionary<ItemType, int> { { ItemType.Cheese, 5 } },
            names.Don
        ));

        missionChain.Add(new Mission(
            "Fuel for the Family",
            new string[] {
                "Don: \"Your cheese tribute was acceptable.\"",
                "Don: \"Swiftwhisker handles our fuel reserves.\"",
                "Don: \"Bring HIM 3 units of diesel.\""
            },
            new Dictionary<ItemType, int> { { ItemType.Diesel, 3 } },
            names.Swiftwhisker
        ));

        missionChain.Add(new Mission(
            "Fortify the Borough",
            new string[] {
                "Don: \"The roaches grow bold…\"",
                "Don: \"Capo Bricktail reinforces the northwest tunnel.\"",
                "Don: \"Bring HIM 10 scrap metal.\""
            },
            new Dictionary<ItemType, int> { { ItemType.ScrapMetal, 10 } },
            names.CapoBricktail
        ));
    }

    // -----------------------------
    // INTERACTION ENTRY
    // -----------------------------
    public void Interact()
    {
        Interact(name);
    }

    public void Interact(names npc)
    {
        // clamp missionIndex
        if (missionIndex < 0 || missionIndex >= missionChain.Count)
        {
            Debug.LogWarning("No valid mission available.");
            return;
        }

        var mission = missionChain[missionIndex];

        // -----------------------------
        // DON: Only Don can start a mission intro
        // -----------------------------
        if (npc == names.Don)
        {
            // 1) If mission not started -> Don starts the intro
            if (!mission.started)
            {
                mission.started = true;
                // open UI and begin Don's intro dialogue
                StartDialogue(names.Don);
                return;
            }

            // 2) If mission started but intro not complete (player closed early) -> reopen intro where left off
            if (mission.started && !mission.introComplete)
            {
                // reopen dialogue at current dialogueIndex
                StartDialogue(names.Don); // StartDialogue will respect dialogueIndex reset below
                return;
            }

            // 3) If mission started and intro complete but mission not yet completed:
            if (mission.started && mission.introComplete && !mission.isCompleted)
            {
                missionPanel.SetActive(true);
                // If Don is the delivery NPC for this mission, attempt completion,
                // otherwise tell player to talk to the delivery NPC.
                if (mission.deliverTo == names.Don)
                {
                    TryCompleteMission(names.Don);
                }
                else
                {
                    dialogueText.text = $"Don: \"The Family waits. Bring this to {mission.deliverTo}.\"";
                }
                return;
            }

            // 4) If mission already completed
            if (mission.isCompleted)
            {
                missionPanel.SetActive(true);
                dialogueText.text = "Don: \"Good work. We'll be in touch when there is more to do.\"";
                return;
            }
        }

        // -----------------------------
        // DELIVERY NPC: Swiftwhisker or CapoBricktail
        // -----------------------------
        if (npc == mission.deliverTo)
        {
            // If mission hasn't been started yet, tell player to talk to Don
            if (!mission.started)
            {
                missionPanel.SetActive(true);
                dialogueText.text = $"{npc}: \"Don hasn't told me to expect anything. Talk to the Don first.\"";
                return;
            }

            // If intro exists but not complete, instruct player to finish intro with Don
            if (mission.started && !mission.introComplete)
            {
                missionPanel.SetActive(true);
                dialogueText.text = $"{npc}: \"You need to hear Don's orders first. Go see him.\"";
                return;
            }

            // If mission is in progress and introComplete: attempt delivery flow
            if (mission.started && mission.introComplete && !mission.isCompleted)
            {
                // Open a small delivery dialogue and let player press Next to attempt completion
                StartDialogue(npc);
                return;
            }

            // If already completed:
            if (mission.isCompleted)
            {
                missionPanel.SetActive(true);
                dialogueText.text = $"{npc}: \"We've already taken care of that. Speak with Don for new work.\"";
                return;
            }
        }

        // -----------------------------
        // WRONG NPC: Not Don, not delivery target
        // -----------------------------
        missionPanel.SetActive(true);
        dialogueText.text = $"{npc}: \"Kid… I ain't expectin' nothin' from you. Find {mission.deliverTo}.\"";
        StartCoroutine(stopDialogue());
    }

    IEnumerator stopDialogue()
    {
        yield return new WaitForSeconds(2f);
        missionPanel.SetActive(false);
    }
    // -----------------------------
    // DIALOGUE / UI CONTROLS
    // -----------------------------
    // StartDialogue records who we're talking to and opens the UI.
    // For Don: we will walk through mission.dialogues lines (intro). For delivery NPC: show single confirm line then Next => TryComplete.
    void StartDialogue(names npc)
    {
        // record who we're talking to for Next button
        currentTalkingNPC = npc;

        inDialogue = true;
        missionPanel.SetActive(true);

        // For Don's intro we want to iterate through mission.dialogues.
        // Reset dialogueIndex to 0 when starting Don's intro for the first time,
        // but if Don reopened mid-intro we continue where we left off.
        if (npc == names.Don)
        {
            // if this is the very first time we open Don's intro for this mission,
            // ensure dialogueIndex starts at 0; otherwise we continue where left off.
            if (!missionChain[missionIndex].started || dialogueIndex == 0)
                dialogueIndex = 0;

            ShowNextDialogue(npc);
            return;
        }

        // For delivery NPC: present a simple confirm line and wait for Next to attempt completion
        if (npc == missionChain[missionIndex].deliverTo)
        {
            // A standard line before attempting to complete
            dialogueText.text = $"{npc}: \"You got what Don ordered? Let me see.\"";
            return;
        }

        // Fallback safety
        dialogueText.text = $"{npc}: \"...\"";
    }

    // Called by the Next button (UI should call OnNextButtonPressed)
    public void OnNextButtonPressed()
    {
        if (!inDialogue)
            return;

        ShowNextDialogue(currentTalkingNPC);
    }

    // Show next dialogue line depending on who is talking
    public void ShowNextDialogue(names npcTalkingTo)
    {
        var mission = missionChain[missionIndex];

        // Don's intro sequence
        if (npcTalkingTo == names.Don)
        {
            // bounds check
            if (mission.dialogues == null || mission.dialogues.Length == 0)
            {
                // No intro lines — mark intro complete and close
                mission.introComplete = true;
                missionInProgress = true;
                CloseUI();
                return;
            }

            // if more lines remain, show next
            if (dialogueIndex < mission.dialogues.Length)
            {
                dialogueText.text = mission.dialogues[dialogueIndex];
                dialogueIndex++;
                return;
            }

            // If we've reached the end of Don's intro lines:
            mission.introComplete = true;
            missionInProgress = true;

            // show a small confirmation and close UI (player will go fetch)
            dialogueText.text = "Don: \"Good. Get to it. Return when the Family's goods are retrieved.\"";

            // Close UI after a short time or leave open until player closes; here we simply close immediately
            // so the next Interact will go to the correct delivery logic.
            Invoke(nameof(CloseUI), 0.6f); // small delay so text can be read; adjust/remove if undesired
            return;
        }

        // Delivery NPC "Next" pressed -> attempt to complete mission
        if (npcTalkingTo == mission.deliverTo)
        {
            // attempt completion
            TryCompleteMission(npcTalkingTo);
            return;
        }

        // Default fallback
        dialogueText.text = $"{npcTalkingTo}: \"I have nothing else to say.\"";
    }

    // -----------------------------
    // MISSION COMPLETION LOGIC
    // -----------------------------
    void TryCompleteMission(names npc)
    {
        var mission = missionChain[missionIndex];

        // safety
        if (npc != mission.deliverTo)
        {
            dialogueText.text = $"{npc}: \"I ain't expectin' nothin' from you.\"";
            return;
        }

        // check items
        if (!HasAllRequiredItems(mission))
        {
            dialogueText.text = $"{npc}: \"You don't got the goods yet. Scram.\"";
            // keep mission in progress; close UI after showing message
            Invoke(nameof(CloseUI), 0.8f);
            return;
        }

        // consume items & complete
        ConsumeRequiredItems(mission);
        mission.isCompleted = true;

        dialogueText.text = $"{npc}: \"Good work. The Family appreciates it.\"";

        // cleanup mission state and advance to next mission
        // reset flags on the current mission (or you can keep a record if you want)
        mission.started = false;
        mission.introComplete = false;
        missionInProgress = false;

        // increment mission index safely
        if (missionIndex < missionChain.Count - 1)
            missionIndex++;

        // close UI after a short moment
        Invoke(nameof(CloseUI), 0.8f);
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
        inDialogue = false;
        // reset currentTalkingNPC so next open must set it again
        currentTalkingNPC = default;
    }

    public string GetPrompt() => "Press E to talk with the Boss";

    void Update() => THATONE = theone.introComplete;
}
