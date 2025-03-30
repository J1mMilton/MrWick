using UnityEngine;
using UnityEngine.UI;

public class SettingsMusicController : MonoBehaviour
{
    public Slider volumeSlider;   // Assign your UI Slider here.
    public Toggle musicToggle;    // Assign your UI Toggle here.

    private void Start()
    {
        // Set the UI elements to reflect current settings.
        if (MusicManager.Instance != null)
        {
            Debug.Log("hahahahahahha");
            volumeSlider.value = MusicManager.Instance.volume;
            musicToggle.isOn = MusicManager.Instance.audioSource.isPlaying;
        }
    }

    // Called by the slider's OnValueChanged event.
    public void OnVolumeChanged(float newVolume)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(newVolume);
        }
    }

    // Called by the toggle's OnValueChanged event.
    public void OnMusicToggleChanged(bool isOn)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMusic(isOn);
        }
    }
}