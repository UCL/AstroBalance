using TMPro;
using UnityEngine;

public class StarCollectorManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;

    [SerializeField, Tooltip("Countdown timer prefab")]
    private CountdownTimer timer;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    [SerializeField, Tooltip("Star generator script")]
    private StarGenerator starGenerator;

    [SerializeField, Tooltip("Minimum game time limit in seconds")]
    private int minTimeLimit = 60;

    [SerializeField, Tooltip("Maximum game time limit in seconds")]
    private int maxTimeLimit = 180;

    [SerializeField, Tooltip("Length of time window (in seconds) to evaluate player perfomance")]
    private int difficultyWindowSeconds = 10;

    [
        SerializeField,
        Tooltip("% of stars that must be collected in the time window to upgrade star speed")
    ]
    private int speedUpgradePercent = 60;

    [
        SerializeField,
        Tooltip("% of stars that must be collected in the whole game to upgrade the time limit")
    ]
    private int timeLimitUpgradePercent = 60;

    private TextMeshProUGUI winText;
    private int timeLimit;
    private int score; // stars collected over whole game
    private int missed; // stars missed over whole game
    private bool gameActive = true;

    private float windowStart;
    private int scoreInTimeWindow = 0; // stars collected in time window
    private int missedInTimeWindow = 0; // stars missed in time window
    private string saveFilename = "StarCollectorScores";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();

        // Load last game data (if any) from file + choose time limit for this game
        SaveData<StarCollectorData> saveData = new SaveData<StarCollectorData>(saveFilename);
        StarCollectorData lastGameData = saveData.GetLastGameData();
      
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
        timer.StartCountdown(timeLimit);

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

        // If time limit reached, end game
        if (timer.GetTimeRemaining() <= 0)
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
            SaveGameData();
        }
    }

    private void SaveGameData()
    {
        float totalStars = score + missed;
        float percentCollected = ((float)score / totalStars) * 100;

        StarCollectorData data = new StarCollectorData
        {
            timeLimit = timeLimit,
            score = score,
            percentCollected = percentCollected,
        };

        SaveData<StarCollectorData> saveData = new SaveData<StarCollectorData>(saveFilename);
        saveData.AddGameData(data);
        saveData.Save();
    }
}
