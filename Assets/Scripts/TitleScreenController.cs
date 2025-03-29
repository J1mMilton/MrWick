using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Scene_World_01");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("SettingScene");
    }

    public void GoToLeaderboard()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }
}
