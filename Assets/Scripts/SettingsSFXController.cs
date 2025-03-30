using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSFXController : MonoBehaviour
{
    public Slider sfxSlider;

    void Start()
    {
        // Initialize the slider with saved value
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.3f);
    }

    void Update()
    {
        
    }

    public void OnSFXSliderChanged(float newVolume)
    {
        PlayerPrefs.SetFloat("SFXVolume", newVolume);
        PlayerPrefs.Save();
    }

}
