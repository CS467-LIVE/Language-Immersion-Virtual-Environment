using UnityEngine;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    [Header("UI Label")]
    public TMP_Text volumeLabel;   // Assign in Inspector

    private bool muted = false;    // sound starts ON

    void Start()
    {
        AudioListener.volume = 1f; // ensure sound starts ON
        UpdateVolumeLabel();
    }

    public void ToggleVolume()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0f : 1f;
        UpdateVolumeLabel();
    }

    private void UpdateVolumeLabel()
    {
        if (volumeLabel != null)
            volumeLabel.text = muted ? "Unmute" : "Mute";
    }
}
