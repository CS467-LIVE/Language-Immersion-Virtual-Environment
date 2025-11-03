using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
  [Header("NPC Info")]
  public string npcID = "npc_default";
  public string npcName = "NPC";
  public string npcRole = "Citizen";

  private MissionManager missionManager;

  void Start()
  {
    missionManager = FindObjectOfType<MissionManager>();

    // Error if MissionManager not found
    if (missionManager == null)
    {
      Debug.LogError($"[NPC {npcName}] Could not find MissionManager in scene.");
    }
  }

  public void Interact()
  {
    Debug.Log($"[NPC] Player interacting with {npcName}");

    // Check if this NPC is active for current mission
    if (missionManager != null && missionManager.IsNPCActiveForMission(npcID))
    {
      Debug.Log($"[NPC] {npcName} is active for mission!");

      // For now, automatically complete the step
      // Need to do: Start dialogue system here
      missionManager.CompleteCurrentStep();
    }
    else
    {
      Debug.Log($"[NPC] {npcName}: I have nothing for you right now.");
    }
  }
}
