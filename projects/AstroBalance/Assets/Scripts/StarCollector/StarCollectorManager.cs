using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StarCollectorManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public GameObject winScreen;
    public StarGenerator starGenerator;
    public int scoreToWin = 100;
    public int timeLimitSeconds = 120;
    public int speedUpgradeWindowSeconds = 10;
    public int speedUpgradePercent = 60;

    private int score;
    private bool gameActive = true;

    private float windowStart;
    private int scoreInTimeWindow = 0;
    private int missedInTimeWindow = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        scoreText.text = score.ToString();

        windowStart = Time.time;

        StartCoroutine(endAfterTimeLimit());
    }

    // Update is called once per frame
    void Update()
    {
        // At end of time window, assess performance and update the difficulty
        // of the game
        if (Time.time - windowStart >= speedUpgradeWindowSeconds)
        {
            updateDifficulty();
        }
        
    }

    private void updateDifficulty()
    {
        float total = scoreInTimeWindow + missedInTimeWindow;
        float percentCollected = ((float)scoreInTimeWindow / total) * 100;

        Debug.Log("percent collected: " + percentCollected);

        if (percentCollected > speedUpgradePercent)
        {
            starGenerator.increaseSpeed();
        }
        else
        {
            starGenerator.decreaseSpeed();
        }

        windowStart = Time.time;
        scoreInTimeWindow = 0;
        missedInTimeWindow = 0;
    }

    public void updateScore()
    {
        score = score += 1;
        scoreText.text = score.ToString();

        scoreInTimeWindow += 1;

        if (score >= scoreToWin)
        {
            endGame();
        }
    }

    public void updateMisses()
    {
        missedInTimeWindow = missedInTimeWindow += 1;
    }

    public bool isGameActive()
    {
        return gameActive;
    }

    IEnumerator endAfterTimeLimit()
    {
        yield return new WaitForSeconds(timeLimitSeconds);
        endGame();
    }

    private void endGame()
    {
        if (gameActive)
        {
            gameActive = false;
            starGenerator.stopGeneration();
            winScreen.SetActive(true);
        }
    }
}
