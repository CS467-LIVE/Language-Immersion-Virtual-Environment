using UnityEngine;
using System.Collections.Generic; // List, Dictionary

// Manages mission state and progression for multi-step missions.
// Tracks which NPCs are active based on current mission step.
// Integrates with NPCInteractable to enable/disable interactions.
public class MissionManager : MonoBehaviour
{
  public enum MissionState { NotStarted, InProgress, WaitingForNextStep, Completed }

  // Represents a complete mission with multiple steps.
  // Example: "Report Lost Item" mission has 2 steps (citizen > police)
  [System.Serializable]
  public class Mission
  {
    public string missionID;  // Unique ID for the mission
    public string missionName;  // Name of the mission
    public List<MissionStep> steps; // Steps in the mission
    public int currentStepIndex = 0;  // Which step the player is on
    public MissionState state = MissionState.NotStarted;
  }

  // Represents one step in a mission.
  // Each step requires talking to a specific NPC.
  [System.Serializable]
  public class MissionStep
  {
    public string stepID;  // Unique ID for the step
    public string npcID;  // NPC involved in this step
    public string objectiveText;  // What player should do
    public bool completed = false;  // If this step is completed
  }

  [Header("Mission Configuration")]
  public List<Mission> missions = new List<Mission>();  // All available missions

  private Mission currentMission; // Currently active mission

  void Start()
  {
    InitializeMissions();
  }

  // Creates mission data and starts the first mission.
  // In the future, this data could come from JSON files or Philips's backend.
  void InitializeMissions()
  {
    // Create the lost item mission
    Mission lostItemMission = new Mission
    {
      missionID = "report_lost_item",
      missionName = "Report a Lost Item",
      steps = new List<MissionStep>
            {
                // Step 1: Find and talk to distressed citizen
                new MissionStep
                {
                    stepID = "find_citizen",
                    npcID = "distressed_citizen",
                    objectiveText = "Find the distressed citizen"
                },
                // Step 2: Report to police officer
                new MissionStep
                {
                    stepID = "report_to_police",
                    npcID = "police_officer",
                    objectiveText = "Report to police station"
                }
            }
    };

    // Add mission to list and start it
    missions.Add(lostItemMission);
    StartMission("report_lost_item");
  }

  // Starts a mission by its ID
  // Param name = "missionID": Unique ID of the mission to start
  public void StartMission(string missionID)
  {
    // Find mission in list
    currentMission = missions.Find(m => m.missionID == missionID);
    if (currentMission != null)
    {
      // Mark mission as in progress
      currentMission.state = MissionState.InProgress;
      Debug.Log($"[Mission] Started: {currentMission.missionName}");
      UpdateObjective();
    }
  }

  // Checks if the given NPC is active for the current mission step
  // Param name = "npcID": Unique ID of the NPC to check
  // Returns: True if NPC is active for current mission step
  public bool IsNPCActiveForMission(string npcID)
  {
    if (currentMission == null)
        return false;
    
    MissionStep currentStep = GetCurrentStep();
    return currentStep != null && currentStep.npcID == npcID;
  }

  // Gets the current mission step
  // Returns: Current MissionStep or null if none
  public MissionStep GetCurrentStep()
  {
    // No active mission or all steps completed
    if (currentMission == null || currentMission.currentStepIndex >= currentMission.steps.Count)
      return null;

    return currentMission.steps[currentMission.currentStepIndex];
  }

  // Marks the current mission step as completed and advances to the next step
  // Called by NPCInteractable when player interacts with the correct NPC
  public void CompleteCurrentStep()
  {
    MissionStep currentStep = GetCurrentStep();
    if (currentStep != null)
    {
      // Mark step as completed
      currentStep.completed = true;
      Debug.Log($"[Mission] Step completed: {currentStep.stepID}");

      // Move to next step
      currentMission.currentStepIndex++;

      // Check if that was the last step
      if (currentMission.currentStepIndex >= currentMission.steps.Count)
      {
        CompleteMission();
      }
      else
      {
        // More steps to go
        currentMission.state = MissionState.WaitingForNextStep;
        // Debug.Log($"[Mission] Next step: {GetCurrentStep().objectiveText}");
        UpdateObjective();
      }
    }
  }

  // Called when all mission steps are completed
  // Handles mission completion logic (rewards)
  void CompleteMission()
  {
    currentMission.state = MissionState.Completed;
    Debug.Log($"[Mission] COMPLETED: {currentMission.missionName}");
    // Need to do: Trigger rewards, next mission, completion UI, etc.
  }

  // Updates the objective text that should be displayed to the player
  // In the future, this would update the UI
  void UpdateObjective()
  {
    MissionStep step = GetCurrentStep();
    if (step != null)
    {
      Debug.Log($"[Mission] Objective: {step.objectiveText}");
      // Need to do: Update UI with objective text
    }
  }

  // Gets the current objective text for UI display
  // Returns: Objective text of current mission step
  public string GetCurrentObjective()
  {
    MissionStep step = GetCurrentStep();
    return step != null ? step.objectiveText : "No active mission";
  }
}
