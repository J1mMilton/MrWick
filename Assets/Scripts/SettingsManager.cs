using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Toggle bgmToggle;
    //public Toggle sfxToggle;
    public Dropdown difficultyDropdown;

    void Start()
    {
        // Load saved settings
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bgmToggle.isOn = PlayerPrefs.GetInt("BGMOn", 1) == 1;
        // sfxToggle.isOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        difficultyDropdown.value = PlayerPrefs.GetInt("Difficulty", 1); // 0: Easy, 1: Normal, 2: Hard
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetInt("BGMOn", bgmToggle.isOn ? 1 : 0);
        // PlayerPrefs.SetInt("SFXOn", sfxToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Difficulty", difficultyDropdown.value);
        PlayerPrefs.Save();

        // Return to main menu
        SceneManager.LoadScene("Scene_Main_Menu_01");
    }
}