using UnityEngine;
using Systems.Events;

public class PhoneBoothInteractable : MonoBehaviour
{
  [Header("Phone Booth Info")]
  [Tooltip("The event ID that will be raised when player uses this phone booth")]
  public string eventID = "PHONEBOOTH_USED";

  [Header("Audio (Optional)")]
  [Tooltip("Sound to play when player uses phone booth")]
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
      audioSource.spatialBlend = 1f;
    }
  }

  /// Called when player presses E near this phone booth
  public void Interact()
  {
    Debug.Log($"[PhoneBooth] Player using phone booth at {gameObject.name}");

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

    // TODO: Backend should open conversation UI here for taxi calling
    // Backend will handle conversation validation and raise TAXI_CALLED_CORRECT event
  }
}
