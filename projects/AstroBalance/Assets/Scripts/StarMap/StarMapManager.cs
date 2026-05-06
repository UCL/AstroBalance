using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StarMapManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;

    [SerializeField, Tooltip("Text mesh pro object for order text i.e. same vs opposite")]
    private TextMeshProUGUI orderText;

    [
        SerializeField,
        Tooltip(
            "Number of maximum score games (in a row) required to upgrade from small to large constellation"
        )
    ]
    private int maxScoreGames = 2;

    [SerializeField, Tooltip("Small constellation prefab")]
    private Constellation smallConstellation;

    [SerializeField, Tooltip("Large constellation prefab")]
    private Constellation largeConstellation;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    private TextMeshProUGUI winText;
    private bool gameActive = true;
    private int maxCorrectSequenceLength = 0; // maximum length of sequence repeated correctly
    private string saveFilename = "StarMapScores";
    private RepeatOrder chosenOrder;
    private Constellation chosenConstellation;
    private ConstellationSize constellationSize;
    private List<StarMapData> gameData = new List<StarMapData>(); // Each item is data on a single 'trial' i.e. a single sequence and guess
    private string gameStartTime; // the overall game start time, in the format needed for the save data

    public enum RepeatOrder
    {
        Same,
        Opposite,
    }

    public enum ConstellationSize
    {
        Small,
        Large,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();

        ChooseConstellationSize();
        SpawnConstellation();

        // Randomly choose forward or reverse direction
        Array orders = Enum.GetValues(typeof(RepeatOrder));
        chosenOrder = (RepeatOrder)orders.GetValue(UnityEngine.Random.Range(0, orders.Length));

        orderText.text = "Repeat in " + chosenOrder.ToString().ToLower() + " order";

        // Record game start time, so it can be used in all trial save data
        StarMapData data = new();
        data.LogEndTime();
        gameStartTime = data.startTime;

        chosenConstellation.ShowNewSequence(chosenOrder);
    }

    /// <summary>
    /// Load previous game data (if any), and choose constellation size based on previous
    /// performance.
    /// </summary>
    private void ChooseConstellationSize()
    {
        SaveGameData<StarMapData> saveData = new(saveFilename);
        IEnumerable<StarMapData> lastNSessionsData = saveData.GetLastNCompleteSessions(
            maxScoreGames
        );
        int smallConstellationMaxLength = smallConstellation.GetNumberOfStars();

        // First time playing the game - start with the small constellation
        if (lastNSessionsData.Count() == 0)
        {
            constellationSize = ConstellationSize.Small;
            return;
        }
        // Once upgraded to the large constellation, stay at the large constellation
        else if (lastNSessionsData.Last().constellationSize == ConstellationSize.Large.ToString())
        {
            constellationSize = ConstellationSize.Large;
            return;
        }

        // Otherwise, loop through session data to determine if there have been enough max score games to upgrade.
        // Note: StarMap saves one row per trial, so there will be multiple rows per game session.
        int nMaxGames = 0;
        List<int> sessionNumbers = new List<int>();
        foreach (StarMapData data in lastNSessionsData)
        {
            bool newSession = !sessionNumbers.Contains(data.sessionNumber);
            if (newSession && data.maxSpan == smallConstellationMaxLength)
            {
                nMaxGames++;
            }

            if (newSession)
            {
                sessionNumbers.Add(data.sessionNumber);
            }
        }

        if (nMaxGames >= maxScoreGames)
        {
            constellationSize = ConstellationSize.Large;
        }
        else
        {
            constellationSize = ConstellationSize.Small;
        }
    }

    private void SpawnConstellation()
    {
        GameObject constellationToInstantiate;
        if (constellationSize == ConstellationSize.Small)
        {
            constellationToInstantiate = smallConstellation.gameObject;
        }
        else
        {
            constellationToInstantiate = largeConstellation.gameObject;
        }

        chosenConstellation = Instantiate(constellationToInstantiate).GetComponent<Constellation>();
    }

    // Update is called once per frame
    void Update() { }

    private int GetNextTrialNumber()
    {
        return gameData.Count() == 0 ? 1 : gameData.Last().trialNumber + 1;
    }

    /// <summary>
    /// Update score (and associated data) after the player guesses a sequence.
    /// </summary>
    /// <param name="guessCorrect">Whether the player guessed the sequence correctly</param>
    /// <param name="sequenceLength">length of the guessed sequence</param>
    /// <param name="afterDowngrade">whether this is after a downgrade in length due to incorrect guesses</param>
    /// <param name="guessTime">total time in seconds the player took to guess</param>
    public void UpdateScore(
        bool guessCorrect,
        int sequenceLength,
        bool afterDowngrade,
        float guessTime
    )
    {
        // Populate save data for this trial
        StarMapData trialData = new StarMapData();
        trialData.trialNumber = GetNextTrialNumber();
        trialData.responseCorrect = guessCorrect;
        trialData.sequenceLength = sequenceLength;
        trialData.responseTimeSeconds = guessTime;
        gameData.Add(trialData);

        // Update score text, and end condition if guess was correct
        if (guessCorrect)
        {
            if (sequenceLength > maxCorrectSequenceLength)
            {
                maxCorrectSequenceLength = sequenceLength;
            }

            scoreText.text = maxCorrectSequenceLength.ToString();

            // game ends when we reach the max number of stars, or when we guess correctly
            // after the sequence length having been reduced due to incorrect guesses
            if (sequenceLength == chosenConstellation.GetNumberOfStars() || afterDowngrade)
            {
                EndGame();
            }
        }
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

            winText.text =
                "Congratulations! \n \n You matched " + maxCorrectSequenceLength + " stars";
            winScreen.SetActive(true);
            SaveGameData(true);
        }
    }

    private void OnDestroy()
    {
        // If the scene is exited early (e.g. with the exit button), then save this
        // partial game's data
        if (gameActive)
        {
            // Add data for this partial (un-finished) trial
            StarMapData trialData = new StarMapData();
            trialData.trialNumber = GetNextTrialNumber();
            trialData.responseCorrect = null;
            trialData.responseTimeSeconds = null;
            trialData.sequenceLength = chosenConstellation.GetCurrentSequenceLength();
            gameData.Add(trialData);

            SaveGameData(false);
        }
    }

    private void SaveGameData(bool gameComplete)
    {
        if (gameData.Count() == 0)
        {
            return;
        }

        // Log end time on first item - this end time will be copied to all
        // trial data
        gameData.ElementAt(0).LogEndTime();
        string endTime = gameData.ElementAt(0).endTime;

        SaveGameData<StarMapData> saveData = new(saveFilename);

        int sessionNumber = CaptureSessionData.CurrentSessionNumber();
        int gameNumber = saveData.GetNextGameNumber();

        // Populate data that is common across all trials
        foreach (StarMapData trialData in gameData)
        {
            trialData.sessionNumber = sessionNumber;
            trialData.gameNumber = gameNumber;
            trialData.startTime = gameStartTime;
            trialData.endTime = endTime;
            trialData.gameCompleted = gameComplete;
            trialData.totalNumberTrials = gameData.Count();
            trialData.maxSpan = maxCorrectSequenceLength;
            trialData.constellationSize = constellationSize.ToString();

            if (chosenOrder == RepeatOrder.Same)
            {
                trialData.sequenceType = "Forward";
            }
            else
            {
                trialData.sequenceType = "Backward";
            }
        }
        saveData.Save(gameData);

        // Update save data for this session
        if (gameComplete)
        {
            CaptureSessionData.MarkGameAsComplete("nCompleteStarMapGames");
        }
    }
}
