using UnityEngine;
using Systems.Events;

public class MailboxInteractable : MonoBehaviour
{
  [Header("Mailbox Info")]
  [Tooltip("The event ID that will be raised when player uses this mailbox")]
  public string eventID = "MAILBOX_USED";

  [Header("Audio (Optional)")]
  [Tooltip("Sound to play when player uses mailbox")]
  public AudioClip interactionSound;

  [Tooltip("Volume for interaction sound (0-1)")]
  [Range(0f, 1f)]
  public float interactionVolume = 0.7f;

  private AudioSource audioSource;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.playOnAwake = false;
      audioSource.spatialBlend = 1f; // 3D sound
    }
  }

  /// Called when player presses E near this mailbox
  public void Interact()
  {
    Debug.Log($"[Mailbox] Player mailed letter at {gameObject.name}");

    if (interactionSound != null && audioSource != null)
    {
      audioSource.PlayOneShot(interactionSound, interactionVolume);
    }

    // Raise event for mission system
    GameEvents.Raise(new GameEvent
    {
      type = "Custom",
      subjectId = eventID,
      amount = 1
    });
  }
}
