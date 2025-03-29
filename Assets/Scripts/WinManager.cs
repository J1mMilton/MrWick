using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // If using TextMeshPro

public class WinManager : MonoBehaviour
{
    public Text finalScoreText;
    public Text finalTimeText;
    public TMP_InputField nameInputField; // Assign in Inspector

    private string leaderboardPath;

    private void Start()
    {
        leaderboardPath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
    }

    private void Update()
    {
        int score = GameData.Instance.score;
        float time = GameData.Instance.time;

        finalScoreText.text = "Your Score: " + score;
        finalTimeText.text = "Your Time: " + FormatTime(time);
    }

    public void SubmitScore()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName))
            playerName = "Anonymous";

        int score = GameData.Instance.score;
        float time = GameData.Instance.time;
        string formattedTime = FormatTime(time);

        // Load existing leaderboard or create new one
        LeaderboardManager.LeaderboardData data = new LeaderboardManager.LeaderboardData();
        if (File.Exists(leaderboardPath))
        {
            string json = File.ReadAllText(leaderboardPath);
            data = JsonUtility.FromJson<LeaderboardManager.LeaderboardData>(json);
        }

        // Add new entry
        var newEntry = new LeaderboardManager.ScoreEntry
        {
            playerName = playerName,
            score = score,
            time = formattedTime
        };
        data.entries.Add(newEntry);

        // Save to file
        string newJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(leaderboardPath, newJson);

        Debug.Log($"Score saved: {playerName} - {score} - {formattedTime}");

        // Optional: load leaderboard scene or show confirmation
        // SceneManager.LoadScene("LeaderboardScene");
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
