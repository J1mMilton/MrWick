using UnityEngine;
using UnityEngine.SceneManagement;

public class FailureUI : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("Scene_World_01");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Scene_Main_Menu_01");
    }
}