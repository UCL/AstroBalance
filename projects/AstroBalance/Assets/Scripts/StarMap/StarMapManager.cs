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
    private int score = 0;
    private int maxSequenceLength = 0; // maximum length of sequence repeated correctly
    private string saveFilename = "StarMapScores";
    private RepeatOrder chosenOrder;
    private Constellation chosenConstellation;
    private ConstellationSize constellationSize;
    private StarMapData gameData;

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

        gameData = new StarMapData();
        chosenConstellation.ShowNewSequence(chosenOrder);
    }

    /// <summary>
    /// Load previous game data (if any), and choose constellation size based on previous
    /// performance.
    /// </summary>
    private void ChooseConstellationSize()
    {
        SaveData<StarMapData> saveData = new(saveFilename);
        IEnumerable<StarMapData> lastNGamesData = saveData.GetLastNCompleteGamesData(maxScoreGames);
        int smallConstellationMaxLength = smallConstellation.GetNumberOfStars();

        // We haven't played enough games, to get maxScoreGames in a row
        if (lastNGamesData.Count() < maxScoreGames)
        {
            constellationSize = ConstellationSize.Small;
        }
        // Once upgraded to the large constellation, stay at the large constellation
        else if (lastNGamesData.Last().constellationSize == ConstellationSize.Large.ToString())
        {
            constellationSize = ConstellationSize.Large;
        }
        // Otherwise upgrade if have enough maxScoreGames
        else
        {
            int nMaxGames = 0;
            foreach (StarMapData data in lastNGamesData)
            {
                if (data.maxSequenceLength == smallConstellationMaxLength)
                {
                    nMaxGames++;
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

    /// <summary>
    /// Increase score (successfully guessed sequences) by one.
    /// </summary>
    /// <param name="sequenceLength">length of the guessed sequence</param>
    /// <param name="afterDowngrade">whether this is after a downgrade in length due to incorrect guesses</param>
    public void UpdateScore(int sequenceLength, bool afterDowngrade)
    {
        score += 1;
        scoreText.text = score.ToString();

        if (sequenceLength > maxSequenceLength)
        {
            maxSequenceLength = sequenceLength;
        }

        // game ends when we reach the max number of stars, or when we guess correctly
        // after the sequence length having been reduced due to incorrect guesses
        if (sequenceLength == chosenConstellation.GetNumberOfStars() || afterDowngrade)
        {
            EndGame();
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

            winText.text = "Congratulations! \n \n You matched " + score + " sequences";
            winScreen.SetActive(true);
            SaveGameData();
        }
    }

    private void SaveGameData()
    {
        gameData.gameCompleted = true;
        gameData.nSequencesRepeated = score;
        gameData.maxSequenceLength = maxSequenceLength;
        gameData.repeatOrder = chosenOrder.ToString();
        gameData.constellationSize = constellationSize.ToString();
        gameData.LogEndTime();

        SaveData<StarMapData> saveData = new(saveFilename);
        saveData.SaveGameData(gameData);
    }
}
