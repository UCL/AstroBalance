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

    [SerializeField, Tooltip("Game time limit in seconds")]
    private int timeLimit = 120;

    private int score;
    private TextMeshProUGUI winText;
    private bool gameActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        score = 0;
        scoreText.text = score.ToString();
        timer.StartCountdown(timeLimit);
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
        }
    }
}
