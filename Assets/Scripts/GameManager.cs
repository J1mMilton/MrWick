using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void SaveToLeaderboard(string name, int score, string time)
    {
        string path = Path.Combine(Application.persistentDataPath, "leaderboard.json");

        LeaderboardManager.LeaderboardData data;
        if (File.Exists(path))
            data = JsonUtility.FromJson<LeaderboardManager.LeaderboardData>(File.ReadAllText(path));
        else
            data = new LeaderboardManager.LeaderboardData();

        data.entries.Add(new LeaderboardManager.ScoreEntry { playerName = name, score = score, time = time });

        File.WriteAllText(path, JsonUtility.ToJson(data));
    }

    //When player wins, set these up
    // PlayerPrefs.SetInt("FinalScore", playerScore);
    // PlayerPrefs.Save();
    // SceneManager.LoadScene("VictoryScene");

    
}
