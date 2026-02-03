using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StarSeekManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;

    [SerializeField, Tooltip("Countdown timer prefab")]
    private CountdownTimer timer;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    [SerializeField, Tooltip("Minimum game time limit in seconds")]
    private int minTimeLimit = 60;

    [SerializeField, Tooltip("Maximum game time limit in seconds")]
    private int maxTimeLimit = 180;

    [SerializeField, Tooltip("Time limit increase if timeLimitUpgradeRate is met")]
    private int timeLimitIncrement = 60;

    [
        SerializeField,
        Tooltip(
            "(number of stars collected / game time limit) i.e. average stars collected per second - must be above this value to increase the time limit of future games."
        )
    ]
    private float timeLimitUpgradeRate = 0.3f;

    [
        SerializeField,
        Tooltip(
            "Number of games in a row that must meet timeLimitUpgradeRate to increase the time limit"
        )
    ]
    private int nGamesToUpgrade = 3;

    private int score;
    private int timeLimit;
    private TextMeshProUGUI winText;
    private bool gameActive = true;
    private StarSeekData gameData;
    private string saveFilename = "StarSeekScores";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        ChooseGameTimeLimit();

        score = 0;
        scoreText.text = score.ToString();
        gameData = new StarSeekData();
        timer.StartCountdown(timeLimit);
    }

    /// <summary>
    /// Load previous game data (if any), and choose time limit for this game based
    /// on prior perfomance.
    /// </summary>
    private void ChooseGameTimeLimit()
    {
        SaveData<StarSeekData> saveData = new(saveFilename);
        IEnumerable<StarSeekData> lastNGamesData = saveData.GetLastNGamesData(nGamesToUpgrade);

        if (lastNGamesData.Count() == 0)
        {
            SetTimeLimit(minTimeLimit);
            return;
        }
        else if (lastNGamesData.Count() < nGamesToUpgrade)
        {
            SetTimeLimit(lastNGamesData.Last().timeLimitSeconds);
            return;
        }

        // Upgrade if all the last n games have the same time limit + meet the upgrade
        // rate. If it's a mix of time limits, then we haven't played enough games at
        // this level yet to progress.
        int nGamesAtUpgradeRate = 0;
        bool allSameTimeLimit = true;
        int lastTimeLimit = lastNGamesData.Last().timeLimitSeconds;

        foreach (StarSeekData data in lastNGamesData)
        {
            float starRate = (float)data.nStarsCollected / (float)data.timeLimitSeconds;

            if (data.timeLimitSeconds != lastTimeLimit)
            {
                allSameTimeLimit = false;
                break;
            }

            if (starRate >= timeLimitUpgradeRate)
            {
                nGamesAtUpgradeRate++;
            }
        }

        if (allSameTimeLimit && nGamesAtUpgradeRate >= nGamesToUpgrade)
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
        // If time limit reached, end game
        if (timer.GetTimeRemaining() <= 0)
        {
            EndGame();
        }
    }

    /// <summary>
    /// Increase score (collected stars) by one.
    /// </summary>
    public void UpdateScore()
    {
        score += 1;
        scoreText.text = score.ToString();
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

            winText.text = "Congratulations! \n \n You collected " + score + " stars";
            winScreen.SetActive(true);
            SaveGameData();
        }
    }

    private void SaveGameData()
    {
        gameData.gameCompleted = true;
        gameData.timeLimitSeconds = timeLimit;
        gameData.nStarsCollected = score;
        gameData.LogEndTime();

        SaveData<StarSeekData> saveData = new(saveFilename);
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
