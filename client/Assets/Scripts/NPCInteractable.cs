using UnityEngine;
using Systems.Events;

public class NPCInteractable : MonoBehaviour
{
    [Header("NPC Info")]
    [Tooltip("NPC's role (e.g., Vendor, Police Officer, Citizen)")]
    public string npcRole = "Citizen";

    [Header("Mission")]
    [Tooltip("ID used by missions (e.g., NPC_SARAH, MAILBOX_1). If empty, falls back to conversation.npcId or GameObject name.")]
    public string subjectIdOverride;

    [Tooltip("For objects without conversation: raise a custom event instead of opening dialogue (e.g., 'Deposit' for mailbox)")]
    public string customEventName;

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

        // Auto-assign DialogueUI if missing, but don't spam console
        if (dialogueUI == null)
        {
            dialogueUI = FindObjectOfType<NpcDialogueUI>();
            if (dialogueUI == null)
            {
                Debug.LogError("[NPCInteractable] No NpcDialogueUI found in scene! Dialogue will not open.");
            }
            // (Removed noisy auto-link success log)
        }
    }

    /// Called when player presses E near this NPC
    public void Interact()
    {
        // Keep this — helpful but not spammy
        //Debug.Log($"[NPC] Player interacting with {conversation.npcName}");

        // Play interaction sound
        if (interactionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactionSound, interactionVolume);
        }

        string subjectId =
            !string.IsNullOrWhiteSpace(subjectIdOverride)
                ? subjectIdOverride
                : (conversation != null && !string.IsNullOrWhiteSpace(conversation.npcId)
                    ? conversation.npcId
                    : name); // fallback to GameObject name

        //Debug.Log($"[NPC] Using subjectId: {subjectId}");
        // Raise event for mission system
        GameEvents.Raise(new GameEvent
        {
            type = "Interacted",
            subjectId = subjectId,
            amount = 1
        });

        // If this is a non-conversational object with a custom event, raise it
        if (!string.IsNullOrWhiteSpace(customEventName) && conversation == null)
        {
            GameEvents.Raise(new GameEvent
            {
                type = "Custom",
                subjectId = subjectId,
                amount = 1
            });

            // Keep: useful for debugging only rare custom events
            Debug.Log($"[NPC] Raised custom event for {subjectId}");

            return; // Don't open dialogue for custom event objects
        }

        // Trigger camera movement if InteractIndicator is present
        InteractIndicator indicator = GetComponent<InteractIndicator>();
        if (indicator != null)
        {
            indicator.BeginConversationCameraMove();
        }

        // --- START CONVERSATION ---
        if (dialogueUI != null && conversation != null)
        {
            dialogueUI.SetActiveNpc(conversation);
        }
        else if (conversation != null)
        {
            Debug.LogWarning($"[NPC] {name} missing dialogueUI reference");
        }
    }
}
