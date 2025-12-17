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
            UpdateDifficulty();
        }

        // If time limit reached, end game
        if (Time.time - gameStart > timeLimit)
        {
            EndGame();
        }
        
    }

    /// <summary>
    /// Update the difficulty of the game based on player performance.
    /// 
    /// Star speed and time limit are increased when the player is doing well,
    /// and decreased when they aren't.
    /// </summary>
    private void UpdateDifficulty()
    {
        // Percent of stars collected in the time window (i.e. the last n seconds)
        float total = scoreInTimeWindow + missedInTimeWindow;
        float percentCollected = ((float)scoreInTimeWindow / total) * 100;

        if (percentCollected > difficultyUpgradePercent)
        {
            Debug.Log("Increasing difficulty");
            starGenerator.IncreaseSpeed();
            UpdateTimeLimit(difficultyWindowSeconds);
        }
        else
        {
            Debug.Log("Decreasing difficulty");
            starGenerator.DecreaseSpeed();
            UpdateTimeLimit(-difficultyWindowSeconds);
        }

        windowStart = Time.time;
        scoreInTimeWindow = 0;
        missedInTimeWindow = 0;
    }

    private void UpdateTimeLimit(int increment)
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
    }

    /// <summary>
    /// Increase score (collected stars) by one.
    /// </summary>
    public void UpdateScore()
    {
        score = score += 1;
        scoreText.text = score.ToString();

        scoreInTimeWindow += 1;

        if (score >= scoreToWin)
        {
            EndGame();
        }
    }

    /// <summary>
    /// Increase misses (missed stars) by one.
    /// </summary>
    public void UpdateMisses()
    {
        missedInTimeWindow = missedInTimeWindow += 1;
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    private void EndGame()
    {
        if (gameActive)
        {
            gameActive = false;
            starGenerator.StopGeneration();
            winScreen.SetActive(true);
        }
    }
}
