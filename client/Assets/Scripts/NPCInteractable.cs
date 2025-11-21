using UnityEngine;
using Systems.Events;

public class NPCInteractable : MonoBehaviour
{
  [Header("NPC Info")]
  [Tooltip("Unique identifier for this NPC (e.g., NPC_VENDOR_HOTDOG)")]
  public string npcID = "npc_default";

  [Tooltip("Display name of the NPC")]
  public string npcName = "NPC";

  [Tooltip("NPC's role (e.g., Vendor, Police Officer, Citizen)")]
  public string npcRole = "Citizen";

  [Header("Conversation")]
  [Tooltip("Placeholder dialogue text (will be replaced by backend AI)")]
  [TextArea(2, 4)]
  public string dialogueText = "Hello!";

  [Header("Audio")]
  [Tooltip("Sound to play when player interacts with NPC")]
  public AudioClip interactionSound;

  [Tooltip("Volume for interaction sound (0-1)")]
  [Range(0f, 1f)]
  public float interactionVolume = 0.7f;

  private AudioSource audioSource;

  void Start()
  {
    // Get or create AudioSource component
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.playOnAwake = false;
      audioSource.spatialBlend = 1f; // 3D sound
    }
  }

  /// Called when player presses E near this NPC
  public void Interact()
  {
    Debug.Log($"[NPC] Player interacting with {npcName}");

    // Play interaction sound
    if (interactionSound != null && audioSource != null)
    {
      audioSource.PlayOneShot(interactionSound, interactionVolume);
    }

    // Raise event for mission system
    GameEvents.Raise(new GameEvent
    {
      type = "TalkedTo",
      subjectId = npcID,
      amount = 1
    });

    // Start backend AI conversation here
    // For now, just show that interaction happened
    Debug.Log($"[NPC] {npcName}: {dialogueText}");
  }
}
