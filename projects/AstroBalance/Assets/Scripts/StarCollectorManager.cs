using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StarCollectorManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public GameObject winScreen;
    private TextMeshProUGUI winText;
    public StarGenerator starGenerator;
    private StarCollectorData starCollectorData;
    
    public int minTimeLimit = 60;
    public int maxTimeLimit = 180;
    public int difficultyWindowSeconds = 10;
    public int difficultyUpgradePercent = 60;

    private int timeLimit;
    private int score;  // stars collected over whole game
    private int missed;  // stars missed over whole game
    private bool gameActive = true;

    private float gameStart;
    private float windowStart;
    private int scoreInTimeWindow = 0;  // stars collected in time window
    private int missedInTimeWindow = 0;  // stars missed in time window

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        starCollectorData = new StarCollectorData();

        score = 0;

        // Load scores (if any) from file + choose time limit for this game
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
    /// Dynamically update the difficulty of the game based on player performance.
    /// 
    /// Star speed is increased when the player is doing well,
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
        }
        else
        {
            Debug.Log("Decreasing difficulty");
            starGenerator.DecreaseSpeed();
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
        score += 1;
        scoreText.text = score.ToString();
        scoreInTimeWindow += 1;
    }

    /// <summary>
    /// Increase misses (missed stars) by one.
    /// </summary>
    public void UpdateMisses()
    {
        missed += 1;
        missedInTimeWindow += 1;
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

            winText.text = "Congratulations! \n \n You collected " + score + " stars";
            winScreen.SetActive(true);

            // Save game details to file
            float totalStars = score + missed;
            float percentCollected = ((float)score / totalStars) * 100;
            starCollectorData.Save(timeLimit, score, percentCollected);
        }
    }
}
