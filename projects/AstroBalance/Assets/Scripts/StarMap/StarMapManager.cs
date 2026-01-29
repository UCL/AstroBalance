using System;
using TMPro;
using UnityEngine;

public class StarMapManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;

    [SerializeField, Tooltip("Text mesh pro object for order text i.e. same vs opposite")]
    private TextMeshProUGUI orderText;

    [SerializeField, Tooltip("Correct sequences required to win")]
    private int winningScore = 5;

    [SerializeField, Tooltip("Constellation of stars")]
    private Constellation constellation;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    private TextMeshProUGUI winText;
    private bool gameActive = true;
    private int score = 0;
    private int maxSequenceLength = 0; // maximum length of sequence repeated correctly
    private string saveFilename = "StarMapScores";
    private RepeatOrder chosenOrder;
    private StarMapData gameData;

    public enum RepeatOrder
    {
        Same,
        Opposite,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();

        // Randomly choose forward or reverse direction
        Array orders = Enum.GetValues(typeof(RepeatOrder));
        chosenOrder = (RepeatOrder) orders.GetValue(UnityEngine.Random.Range(0, orders.Length));

        orderText.text = "Repeat in " + chosenOrder.ToString().ToLower() + " order";

        gameData = new StarMapData();
        constellation.ShowNewSequence(chosenOrder);
    }

    // Update is called once per frame
    void Update() { }

    /// <summary>
    /// Increase score (successfully guessed sequences) by one.
    /// </summary>
    /// <param name="sequenceLength">length of this correctly guessed sequence</param>
    public void UpdateScore(int sequenceLength)
    {
        score += 1;
        scoreText.text = score.ToString();

        if (sequenceLength > maxSequenceLength)
        {
            maxSequenceLength = sequenceLength;
        }

        if (score == winningScore)
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
        gameData.LogEndTime();

        SaveData<StarMapData> saveData = new(saveFilename);
        saveData.SaveGameData(gameData);
    }
}
