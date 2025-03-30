using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    // AudioSource for playing background music.
    public AudioSource audioSource;

    // Different background music clips.
    public AudioClip MainMenuClip;
    public AudioClip level1Clip;
    public AudioClip level2Clip;

    // Optional: default volume.
    [Range(0f, 1f)]
    public float volume = 0.05f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes.
            
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            // Load saved volume from PlayerPrefs; default to the current 'volume' value if not found.
            volume = PlayerPrefs.GetFloat("BGMVolume", volume);
            audioSource.volume = volume;
            
            // Subscribe to scene loaded event so we can change the music automatically.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called whenever a new scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusicForScene(scene.name);
    }

    // Change the background music clip based on scene name.
    public void ChangeMusicForScene(string sceneName)
    {
        // Adjust these names as needed to match your Build Settings.
        if (sceneName.Contains("Scene_World_01"))
        {
            if (level1Clip != null && audioSource.clip != level1Clip)
            {
                audioSource.clip = level1Clip;
                audioSource.Play();
            }
        }
        else if (sceneName.Contains("Scene_World_02"))
        {
            if (level2Clip != null && audioSource.clip != level2Clip)
            {
                audioSource.clip = level2Clip;
                audioSource.Play();
            }
        }
        else if (sceneName.Contains("Scene_Main_Menu_01"))
        {
            if (MainMenuClip != null && audioSource.clip != MainMenuClip)
            {
                audioSource.clip = MainMenuClip;
                audioSource.Play();
            }
        }
        // Otherwise, you can keep the current music or add more cases.
    }

    // Adjust the volume.
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        if (audioSource != null)
            audioSource.volume = volume;
    }

    // Toggle the music on/off.
    public void ToggleMusic(bool isOn)
    {
        if (audioSource != null)
        {
            if (isOn)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }
#endif

    
}
