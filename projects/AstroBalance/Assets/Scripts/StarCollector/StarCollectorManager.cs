using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StarCollectorManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public GameObject winScreen;
    public int scoreToWin = 10;
    public int timeLimitSeconds = 60;
    public int speedUpgradeWindowSeconds = 20;
    public int speedUpgradePercent = 60;

    // temporary - to move
    public StarGenerator starGenerator;

    private int score;
    private bool gameActive = true;

    private float windowStart;
    private int scoreInTimeWindow = 0;
    private int missedInTimeWindow = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        scoreText.text = score.ToString();

        windowStart = Time.time;

        StartCoroutine(endAfterTimeLimit());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - windowStart >= speedUpgradeWindowSeconds)
        {
            // Assess performance + decide if to promote speed
            Debug.Log("score in window: " + scoreInTimeWindow);
            Debug.Log("missed in window: " + missedInTimeWindow);

            double total = scoreInTimeWindow + missedInTimeWindow;
            double percentCollected = ((double) scoreInTimeWindow / total)*100;
            
            Debug.Log("percent: " + percentCollected);

            if (percentCollected > speedUpgradePercent)
            {
                // UPGRADE SPEED
                starGenerator.increaseSpeed();
            } else
            {
                // DOWNGRADE SPEED
            }
            windowStart = Time.time;
            scoreInTimeWindow = 0;
            missedInTimeWindow = 0;
        }
        
    }

    public void updateScore()
    {
        score = score += 1;
        scoreText.text = score.ToString();

        scoreInTimeWindow += 1;

        if (score >= scoreToWin)
        {
            endGame();
        }
    }

    public void updateMisses()
    {
        missedInTimeWindow = missedInTimeWindow += 1;
    }

    public bool isGameActive()
    {
        return gameActive;
    }

    IEnumerator endAfterTimeLimit()
    {
        yield return new WaitForSeconds(timeLimitSeconds);
        endGame();
    }

    private void endGame()
    {
        if (gameActive)
        {
            gameActive = false;
            winScreen.SetActive(true);
        }
    }
}
