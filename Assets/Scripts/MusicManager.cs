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

    public bool musicOn = true;

    // Optional: default volume.
    [Range(0f, 1f)]
    public float volume = 0.05f;

    private void Awake()
    {
        musicOn = PlayerPrefs.GetInt("BGMOn", 1) == 1;
        
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
        AudioClip selectedClip = null;

        if (sceneName.Contains("Scene_World_01"))
            selectedClip = level1Clip;
        else if (sceneName.Contains("Scene_World_02"))
            selectedClip = level2Clip;
        else if (sceneName.Contains("Scene_Main_Menu_01"))
            selectedClip = MainMenuClip;

        if (selectedClip != null && audioSource.clip != selectedClip)
        {
            audioSource.clip = selectedClip;

            if (musicOn) // ← RESPECT THE TOGGLE
                audioSource.Play();
        }
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
        musicOn = isOn;
        PlayerPrefs.SetInt("BGMOn", isOn ? 1 : 0);
        PlayerPrefs.Save();

        if (audioSource != null)
        {
            if (musicOn)
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
