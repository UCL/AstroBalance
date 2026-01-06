using TMPro;
using UnityEngine;

public class StarCollectorManager : MonoBehaviour
{

    [Tooltip("Text mesh pro object for score text")]
    public TextMeshProUGUI scoreText;
    [Tooltip("Text mesh pro object for countdown timer text")]
    public TextMeshProUGUI timerText;
    [Tooltip("Screen shown upon winning the game")]
    public GameObject winScreen;
    [Tooltip("Star generator script")]
    public StarGenerator starGenerator;

    [Tooltip("Minimum game time limit in seconds")]
    public int minTimeLimit = 60;
    [Tooltip("Maximum game time limit in seconds")]
    public int maxTimeLimit = 180;
    [Tooltip("Length of time window (in seconds) to evaluate player perfomance")]
    public int difficultyWindowSeconds = 10;
    [Tooltip("% of stars that must be collected in the time window to upgrade star speed")]
    public int speedUpgradePercent = 60;
    [Tooltip("% of stars that must be collected in the whole game to upgrade the time limit")]
    public int timeLimitUpgradePercent = 60;

    private TextMeshProUGUI winText;
    private StarCollectorData starCollectorData;
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

        // Load last game data (if any) from file + choose time limit for this game
        StarCollectorData.SaveData lastGameData = starCollectorData.Load();
        if (lastGameData == null)
        {
            SetTimeLimit(minTimeLimit);
        } 
        else if (lastGameData.percentCollected > timeLimitUpgradePercent)
        {
            // Increase time limit by 30 seconds vs last game
            SetTimeLimit(lastGameData.timeLimit + 30);
        }
        else
        {
            SetTimeLimit(lastGameData.timeLimit);
        }

        score = 0;
        scoreText.text = score.ToString();
        UpdateTimerText(timeLimit);

        gameStart = Time.time;
        windowStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameActive)
        {
            return;
        }

        // At end of time window, assess performance and update the difficulty
        // of the game
        if (Time.time - windowStart >= difficultyWindowSeconds)
        {
            UpdateDifficulty();
        }

        float elaspedTime = Time.time - gameStart;
        float timeRemaining = timeLimit - elaspedTime;
        UpdateTimerText(timeRemaining);

        // If time limit reached, end game
        if (timeRemaining <= 0)
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

        if (percentCollected > speedUpgradePercent)
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

    private void SetTimeLimit(int limit)
    {
        if (limit > maxTimeLimit)
        {
            timeLimit = maxTimeLimit;
        }
        else if (limit < minTimeLimit)
        {
            timeLimit = minTimeLimit;
        }
        else
        {
            timeLimit = limit;
        }
    }

    private void UpdateTimerText(float secondsLeft)
    {
        if (secondsLeft < 0)
        {
            secondsLeft = 0;
        }

        float minutes = Mathf.FloorToInt(secondsLeft / 60);
        float seconds = Mathf.CeilToInt(secondsLeft % 60);

        // If there's e.g. 59.5 seconds left, we want to display 1:00
        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }

        timerText.text = $"{minutes:00}:{seconds:00}";
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
