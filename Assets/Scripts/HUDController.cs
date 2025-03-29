using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;
    public Text hpText;
    
    private int currentScore;

    private float elapsedTime = 0f;

    void Start()
    {
        currentScore = GameData.Instance.score;
        elapsedTime = GameData.Instance.time;

        scoreText.text = currentScore.ToString();
        timeText.text = Mathf.FloorToInt(elapsedTime).ToString();
    }

    
    void Update()
    {
        // Update time every frame
        elapsedTime += Time.deltaTime;
        timeText.text = Mathf.FloorToInt(elapsedTime).ToString();
        
        GameData.Instance.time = elapsedTime;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString();
        
        GameData.Instance.score = currentScore;
    }

    public void UpdateHP(int hp)
    {
        hpText.text = "HP: " + hp;
    }
}