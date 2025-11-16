using UnityEngine;
using Systems.Events;

public class NPCInteractable : MonoBehaviour
{
  [Header("NPC Info")]
  public string npcID = "npc_default";
  public string npcName = "NPC";
  public string npcRole = "Citizen";

  [Header("Dialogue (optional)")]
  [TextArea(2, 4)]
  public string dialogueText = "Hello!";

  public void Interact()
  {
    Debug.Log($"[NPC] Player interacting with {npcName}");

    // Raise event for mission system (Systems.Missions.MissionManager listens to this)
    GameEvents.Raise(new GameEvent
    {
      type = "TalkedTo",
      subjectId = npcID,
      amount = 1
    });

    // In the future: Start dialogue system here
    // For now, just show that interaction happened
    Debug.Log($"[NPC] {npcName}: {dialogueText}");
  }
}
