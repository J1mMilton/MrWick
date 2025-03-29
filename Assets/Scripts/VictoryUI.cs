using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    public Text scoreText; // Link this in Inspector

    void Start()
    {
        // Display final score (assuming it's stored globally)
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = $"Your Score: {finalScore}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Scene_World_01");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Scene_Main_Menu_01");
    }

    public void ViewLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }
}