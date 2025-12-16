using System.Collections;
using TMPro;
using UnityEngine;

public class StarCollectorManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public GameObject winScreen;
    public int scoreToWin = 10;
    public int timeLimitSeconds = 10;

    public StarGenerator starGenerator;
    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        scoreText.text = score.ToString();

        StartCoroutine(endAfterTimeLimit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScore()
    {
        score = score += 1;
        scoreText.text = score.ToString();

        if (score >= scoreToWin)
        {
            endGame();
        }
    }

    IEnumerator endAfterTimeLimit()
    {
        yield return new WaitForSeconds(timeLimitSeconds);
        endGame();
    }

    private void endGame()
    {
        starGenerator.stopStarGeneration();
        winScreen.SetActive(true);
    }
}
