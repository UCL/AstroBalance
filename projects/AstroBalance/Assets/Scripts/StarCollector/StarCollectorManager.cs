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
    
    public int minTimeLimit = 60;
    public int maxTimeLimit = 180;
    public int difficultyWindowSeconds = 10;
    public int difficultyUpgradePercent = 60;

    private int timeLimit;
    private int score;
    private bool gameActive = true;

    private float gameStart;
    private float windowStart;
    private int scoreInTimeWindow = 0;
    private int missedInTimeWindow = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        timeLimit = minTimeLimit;
        scoreText.text = score.ToString();

        gameStart = Time.time;
        windowStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // At end of time window, assess performance and update the difficulty
        // of the game
        if (Time.time - windowStart >= difficultyWindowSeconds)
        {
            updateDifficulty();
        }

        // If time limit reached, end game
        if (Time.time - gameStart > timeLimit)
        {
            endGame();
        }
        
    }

    private void updateDifficulty()
    {
        float total = scoreInTimeWindow + missedInTimeWindow;
        float percentCollected = ((float)scoreInTimeWindow / total) * 100;

        Debug.Log("percent collected: " + percentCollected);

        if (percentCollected > difficultyUpgradePercent)
        {
            starGenerator.increaseSpeed();
            updateTimeLimit(difficultyWindowSeconds);
        }
        else
        {
            starGenerator.decreaseSpeed();
            updateTimeLimit(-difficultyWindowSeconds);
        }

        windowStart = Time.time;
        scoreInTimeWindow = 0;
        missedInTimeWindow = 0;
    }

    private void updateTimeLimit(int increment)
    {
        int newTimeLimit = timeLimit + increment;
        if (newTimeLimit > maxTimeLimit)
        {
            timeLimit = maxTimeLimit;
        }
        else if (newTimeLimit < minTimeLimit)
        {
            timeLimit = minTimeLimit;
        }
        else
        {
            timeLimit = newTimeLimit;
        }

        Debug.Log("time limit:" + timeLimit);
        Debug.Log("time left:" + (timeLimit - (Time.time - gameStart)));
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
