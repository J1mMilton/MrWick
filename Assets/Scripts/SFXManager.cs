using System;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    [Range(0f, 1f)]
    public float sfxVolume = 0.3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Load saved SFX volume
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", sfxVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
    }

    public void SetVolume(float newVolume)
    {
        sfxVolume = newVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
}