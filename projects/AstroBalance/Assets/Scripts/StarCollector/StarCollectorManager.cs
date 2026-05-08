using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tobii.GameIntegration.Net;
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

    [SerializeField, Tooltip("Time limit increase if timeLimitUpgradePercent is met")]
    private int timeLimitIncrement = 60;

    [
        SerializeField,
        Tooltip(
            "Number of games in a row that must meet timeLimitUpgradePercent to increase the time limit"
        )
    ]
    private int nGamesToUpgrade = 3;

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

    [
        SerializeField,
        Tooltip("Maximum number of head yaw readings to keep in the buffer at one time")
    ]
    private int maxNItemsInBuffer = 100;

    [
        SerializeField,
        Tooltip("The minimum number of head yaw readings needed to calculate a head velocity")
    ]
    private int minNItemsForVelocity = 5;

    [SerializeField, Tooltip("The number of seconds to calculate head yaw velocity over")]
    private float samplingIntervalSeconds = 0.5f;

    private TextMeshProUGUI winText;
    private Tracker tracker;
    private int timeLimit;
    private int score; // stars collected over whole game
    private int missed; // stars missed over whole game
    private bool gameActive = true;

    private float speedWindowStart; // start time of speed upgrade window
    private int scoreInTimeWindow = 0; // stars collected in time window
    private int missedInTimeWindow = 0; // stars missed in time window
    private string saveFilename = "StarCollectorScores";
    private StarCollectorData gameData;

    private float bufferWindowStart; // start time of buffer update window
    private bool outOfRangeInWindow = false; // whether the player went out of range of the tracker during this window
    private HeadAngleBuffer headYawBuffer;
    private List<float> headYawVelocities = new List<float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChooseGameTimeLimit();

        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        tracker = FindFirstObjectByType<Tracker>();
        gameData = new StarCollectorData();
        score = 0;
        scoreText.text = score.ToString();

        headYawBuffer = new HeadAngleBuffer(maxNItemsInBuffer, minNItemsForVelocity);
        timer.StartCountdown(timeLimit);

        speedWindowStart = Time.time;
        bufferWindowStart = Time.time;
    }

    /// <summary>
    /// Load previous game data (if any), and choose time limit for this game based
    /// on prior perfomance.
    /// </summary>
    private void ChooseGameTimeLimit()
    {
        SaveGameData<StarCollectorData> saveData = new(saveFilename);
        IEnumerable<StarCollectorData> lastNGamesData = saveData.GetLastNComplete(nGamesToUpgrade);

        if (lastNGamesData.Count() < nGamesToUpgrade)
        {
            SetTimeLimit(minTimeLimit);
            return;
        }

        // Upgrade if all the last n games have the same time limit + meet the upgrade
        // percent. If it's a mix of time limits, then we haven't played enough games at
        // this level yet to progress.
        int nGamesAtUpgradePercent = 0;
        bool allSameTimeLimit = true;
        int lastTimeLimit = lastNGamesData.Last().timeLimitSeconds;

        foreach (StarCollectorData data in lastNGamesData)
        {
            if (data.timeLimitSeconds != lastTimeLimit)
            {
                allSameTimeLimit = false;
                break;
            }

            if (data.percentStarsCollected > timeLimitUpgradePercent)
            {
                nGamesAtUpgradePercent++;
            }
        }

        if (allSameTimeLimit && nGamesAtUpgradePercent >= nGamesToUpgrade)
        {
            SetTimeLimit(lastTimeLimit + timeLimitIncrement);
        }
        else
        {
            SetTimeLimit(lastTimeLimit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameActive)
        {
            return;
        }

        // Keep track of head yaw angles on every update
        UpdateHeadYawBuffer();

        // Every 'samplingIntervalSeconds', record the velocity averaged over that time period
        if (Time.time - bufferWindowStart >= samplingIntervalSeconds)
        {
            RecordHeadVelocity();
        }

        // At end of time window, assess performance and update the difficulty
        // of the game
        if (Time.time - speedWindowStart >= difficultyWindowSeconds)
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
    /// Add latest head yaw angle to buffer
    /// </summary>
    private void UpdateHeadYawBuffer()
    {
        // If the player goes out of range of the tracker
        if (!tracker.isPlayerDetected())
        {
            outOfRangeInWindow = true;
        }
        else
        {
            HeadPose headPose = tracker.getHeadPose();
            HeadYawItem headYaw = new HeadYawItem(headPose);
            headYawBuffer.addIfNew(headYaw);
        }
    }

    /// <summary>
    /// Record the latest head yaw velocity
    /// </summary>
    private void RecordHeadVelocity()
    {
        // Only record velocities if the player was in range of the tracker for the whole window.
        // (otherwise, if they've been out of range for a while, we may be calculating the speed of quite old data in the buffer)
        if (!outOfRangeInWindow)
        {
            float headVelocity = headYawBuffer.getSpeed(samplingIntervalSeconds);

            // If there aren't enough recorded head yaw angles yet, the returned velocity is zero.
            // We don't want to include these readings in the overall averages.
            if (headVelocity > 0)
            {
                headYawVelocities.Add(headVelocity);
            }
        }

        outOfRangeInWindow = false;
        bufferWindowStart = Time.time;
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
            starGenerator.IncreaseSpeed();
        }
        else
        {
            starGenerator.DecreaseSpeed();
        }

        speedWindowStart = Time.time;
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
            SaveGameData(true);
        }
    }

    private void OnDestroy()
    {
        // If the scene is exited early (e.g. with the exit button), then save this
        // partial game's data
        if (gameActive)
        {
            SaveGameData(false);
        }
    }

    private void SaveGameData(bool gameComplete)
    {
        // Update save data for this game
        gameData.gameCompleted = gameComplete;
        gameData.timeLimitSeconds = timeLimit;
        gameData.gameDurationSeconds = MathsUtilities.RoundToNearestInt(timer.GetElapsedTime());
        gameData.LogEndTime();

        float totalStars = score + missed;
        float percentCollected = ((float)score / totalStars) * 100;
        gameData.nStarsCollected = score;
        gameData.percentStarsCollected = MathsUtilities.RoundTo2DecimalPlaces(percentCollected);
        gameData.adaptiveLevel =
            1 + Mathf.CeilToInt((timeLimit - minTimeLimit) / timeLimitIncrement);
        gameData.finalStarFallSpeed = starGenerator.GetStarSpeed();

        if (headYawVelocities.Count() == 0)
        {
            gameData.headVelocityDegPerSecPeak = 0;
            gameData.headVelocityDegPerSecMean = 0;
            gameData.headVelocityDegPerSecSD = 0;
        }
        else
        {
            gameData.headVelocityDegPerSecPeak = MathsUtilities.RoundTo2DecimalPlaces(
                headYawVelocities.Max()
            );
            float average = headYawVelocities.Average();
            gameData.headVelocityDegPerSecMean = MathsUtilities.RoundTo2DecimalPlaces(average);
            float standardDeviation = Mathf.Sqrt(
                headYawVelocities.Average(v => Mathf.Pow(v - average, 2))
            );
            gameData.headVelocityDegPerSecSD = MathsUtilities.RoundTo2DecimalPlaces(
                standardDeviation
            );
        }

        SaveGameData<StarCollectorData> saveData = new(saveFilename);
        gameData.sessionNumber = CaptureSessionData.CurrentSessionNumber();
        gameData.gameNumber = saveData.GetNextGameNumber();
        saveData.Save(gameData);

        // Update save data for this session
        if (gameComplete)
        {
            CaptureSessionData.MarkGameAsComplete("nCompleteStarCollectorGames");
        }
    }
}
