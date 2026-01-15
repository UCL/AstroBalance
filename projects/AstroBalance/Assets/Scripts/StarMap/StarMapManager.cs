using TMPro;
using UnityEngine;

public class StarMapManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Correct sequences required to win")]
    private int winningScore = 5;

    private bool gameActive = true;

    private int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Increase score (successfully guessed sequences) by one.
    /// </summary>
    public void UpdateScore()
    {
        score += 1;
        scoreText.text = score.ToString();

        if (score == winningScore) {
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

            Debug.Log("Game ends");

            //winText.text = "Congratulations! \n \n You collected " + score + " stars";
            //winScreen.SetActive(true);
        }
    }
}
