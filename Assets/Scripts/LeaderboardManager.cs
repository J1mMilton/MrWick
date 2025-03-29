using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject entryPrefab;
    public Transform entryContainer;

    private string leaderboardPath;

    void Start()
    {
        leaderboardPath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
    }

    void LoadLeaderboard()
    {
        if (!File.Exists(leaderboardPath)) return;

        string json = File.ReadAllText(leaderboardPath);
        LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);

        List<ScoreEntry> sorted = data.entries
            .OrderByDescending(e => e.score)
            .ThenBy(e => TimeSpan.Parse(e.time)) // assumes e.time is "mm:ss"
            .ToList();


        foreach (var entry in sorted)
        {
            GameObject newEntry = Instantiate(entryPrefab, entryContainer);
            newEntry.GetComponentInChildren<Text>().text = $"{entry.playerName} - {entry.score} pts - {entry.time}";
        }
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public List<ScoreEntry> entries = new List<ScoreEntry>();
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
        public string time;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Scene_Main_Menu_01");
    }
    
}