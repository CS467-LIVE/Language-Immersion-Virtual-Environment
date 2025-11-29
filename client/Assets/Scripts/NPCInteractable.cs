using UnityEngine;
using Systems.Events;

public class NPCInteractable : MonoBehaviour
{
    [Header("NPC Info")]
    [Tooltip("Display name of the NPC")]
    public string npcName = "NPC";

    [Tooltip("NPC's role (e.g., Vendor, Police Officer, Citizen)")]
    public string npcRole = "Citizen";

    [Header("Conversation")]
    [Tooltip("Placeholder dialogue text (will be replaced by backend AI)")]
    [TextArea(2, 4)]
    public string dialogueText = "Hello!";

    [Tooltip("Conversation logic for this NPC")]
    public NpcConversation conversation;

    [Tooltip("Reference to the global dialogue UI")]
    public NpcDialogueUI dialogueUI;

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
            subjectId = conversation.npcId,
            amount = 1
        });

        // Trigger camera movement if InteractIndicator is present
        InteractIndicator indicator = GetComponent<InteractIndicator>();
        if (indicator != null)
        {
            indicator.BeginConversationCameraMove();
        }

        if (dialogueUI != null && conversation != null)
        {
            // tell the UI which NPC is now active
            dialogueUI.SetActiveNpc(conversation);
        }
        else
        {
            Debug.LogWarning($"[NPC] {name} missing dialogueUI or conversation reference");
        }
    }
}
