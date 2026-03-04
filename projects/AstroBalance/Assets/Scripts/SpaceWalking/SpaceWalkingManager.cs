using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SpaceWalkingManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;

    [SerializeField, Tooltip("Direction tile manager")]
    private TileManager tileManager;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    [
        SerializeField,
        Tooltip(
            "Screen showing head turn arrows (only at highest difficulty level or when debugHeadTurns is enabled)"
        )
    ]
    private HeadTurnScreen headTurnScreen;

    [SerializeField, Tooltip("Countdown timer prefab")]
    private CountdownTimer timer;

    [SerializeField, Tooltip("Minimum game time limit in seconds")]
    private int minTimeLimit = 60;

    [SerializeField, Tooltip("Maximum game time limit in seconds")]
    private int maxTimeLimit = 180;

    [SerializeField, Tooltip("Time limit increase if upgradeRate is met")]
    private int timeLimitIncrement = 60;

    [
        SerializeField,
        Tooltip(
            "(number of complete steps / game time limit) i.e. average complete steps per second - must be above this value to increase the difficulty of future games."
        )
    ]
    private float upgradeRate = 0.23f; // default set so that e.g. 1 minute requires ~14 complete steps (out and back to the centre)

    [
        SerializeField,
        Tooltip("Number of games in a row that must meet upgradeRate to increase the difficulty")
    ]
    private int nGamesToUpgrade = 3;

    [SerializeField, Tooltip("Seconds until first tile activation")]
    private int activationDelay = 1;

    [
        SerializeField,
        Tooltip(
            "When enabled, head turns will be activated even if the required difficulty level hasn't been reached (useful for debugging purposes)"
        )
    ]
    private bool debugHeadTurns = false;

    private TextMeshProUGUI winText;
    private bool gameActive = true;
    private int score = 0;
    private int timeLimit;
    private SpaceWalkingData gameData;
    private string saveFilename = "SpaceWalkingScores";
    private bool headTurnsActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        ChooseGameDifficulty();

        gameData = new SpaceWalkingData();
        timer.StartCountdown(timeLimit);
        StartCoroutine(StartTileActivation());
    }

    /// <summary>
    /// Load previous game data (if any), and choose time limit + whether head turns are active
    /// for this game based on prior perfomance.
    /// </summary>
    private void ChooseGameDifficulty()
    {
        SaveData<SpaceWalkingData> saveData = new(saveFilename);
        IEnumerable<SpaceWalkingData> lastNGamesData = saveData.GetLastNCompleteGamesData(
            nGamesToUpgrade
        );

        if (debugHeadTurns)
        {
            headTurnsActive = true;
        }
        else
        {
            headTurnsActive = false;
        }

        if (lastNGamesData.Count() < nGamesToUpgrade)
        {
            SetTimeLimit(minTimeLimit);
            return;
        }

        // Once head turns have been activated, they should remain active for all future games
        if (lastNGamesData.Last().headTurnsActive)
        {
            headTurnsActive = true;
        }

        // Upgrade if all the last n games have the same time limit + meet the upgrade
        // rate. If it's a mix of time limits, then we haven't played enough games at
        // this level yet to progress.
        int nGamesAtUpgradeRate = 0;
        bool allSameTimeLimit = true;
        int lastTimeLimit = lastNGamesData.Last().timeLimitSeconds;

        foreach (SpaceWalkingData data in lastNGamesData)
        {
            float stepRate = (float)data.nCompleteSteps / (float)data.timeLimitSeconds;

            if (data.timeLimitSeconds != lastTimeLimit)
            {
                allSameTimeLimit = false;
                break;
            }

            if (stepRate >= upgradeRate)
            {
                nGamesAtUpgradeRate++;
            }
        }

        if (allSameTimeLimit && nGamesAtUpgradeRate >= nGamesToUpgrade)
        {
            if (lastTimeLimit == maxTimeLimit)
            {
                headTurnsActive = true;
            }
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
        // If time limit reached, end game
        if (timer.GetTimeRemaining() <= 0)
        {
            EndGame();
        }
    }

    private IEnumerator StartTileActivation()
    {
        yield return new WaitForSeconds(activationDelay);
        NextTile();
    }

    /// <summary>
    /// Initiate a head turn sequence.
    /// If the correct difficulty level hasn't been reached yet,
    /// this will activate the next tile instead.
    /// </summary>
    public void HeadTurn()
    {
        if (headTurnsActive)
        {
            headTurnScreen.SpawnRandomArrow();
        }
        else
        {
            NextTile();
        }
    }

    /// <summary>
    /// Activate the next tile to step on.
    /// </summary>
    public void NextTile()
    {
        tileManager.ActivateNextTile();
    }

    /// <summary>
    /// Increase score (successfully completed steps) by one,
    /// then try to start a head turn sequence.
    /// </summary>
    public void UpdateScore()
    {
        if (!gameActive)
        {
            return;
        }

        score += 1;
        scoreText.text = score.ToString();
        HeadTurn();
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

            headTurnScreen.gameObject.SetActive(false);
            winText.text = "Congratulations! \n \n You completed " + score + " steps";
            winScreen.SetActive(true);
            SaveGameData();
        }
    }

    private void SaveGameData()
    {
        gameData.gameCompleted = true;
        gameData.timeLimitSeconds = timeLimit;
        gameData.nCompleteSteps = score;
        gameData.headTurnsActive = headTurnsActive;
        gameData.LogEndTime();

        SaveData<SpaceWalkingData> saveData = new(saveFilename);
        saveData.SaveGameData(gameData);
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
}
