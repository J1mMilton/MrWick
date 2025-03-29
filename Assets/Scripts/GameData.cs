using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public int score = 0;
    public float time = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // survive scene load
        }
        else
        {
            Destroy(gameObject); // only one instance allowed
        }
    }
}