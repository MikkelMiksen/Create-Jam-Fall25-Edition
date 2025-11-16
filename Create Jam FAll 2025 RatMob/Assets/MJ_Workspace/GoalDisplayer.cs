using UnityEngine;
using TMPro;

public class GoalDisplayer : MonoBehaviour
{
    [Header("References")]
    public BossMissionGiver missionGiver;      // Assign the BossMissionGiver object
    public TextMeshProUGUI missionText;        // The TMP text to show the mission goal

    private Mission lastMission;

    void Start()
    {
        if (missionGiver == null)
            missionGiver = FindObjectOfType<BossMissionGiver>();

        UpdateMissionText();
    }

    void Update()
    {
        // Refresh only when mission changes (efficient)
        Mission current = missionGiver.missionChain[missionGiver.missionIndex];

        if (current != lastMission)
        {
            lastMission = current;
            UpdateMissionText();
        }
    }

    void UpdateMissionText()
    {
        Mission mission = missionGiver.missionChain[missionGiver.missionIndex];

        if (!missionGiver.missionInProgress)
        {
            missionText.text = "Goal: ";
            return;
        }

        if (mission.isCompleted)
        {
            missionText.text = "Mission Complete!";
            return;
        }

        // Build mission objective line(s)
        if (missionGiver.missionInProgress && mission.introComplete)
        {
            string reqString = "";
            foreach (var req in mission.requiredItems)
                reqString += $"{req.Key}: {req.Value}\n";

            missionText.text =
                $"<b>{mission.title}</b>\n" +
                $"Deliver to: {mission.deliverTo}\n\n" +
                $"Required:\n{reqString}";
        }
    }
}