using System;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class SpaceWalkingManager : MonoBehaviour
{

    //[SerializeField, Tooltip("Text mesh pro object for score text")]
    //private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Complete steps required to win")]
    private int winningScore = 5;
    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;
    [SerializeField, Tooltip("Direction tile manager")]
    private TileManager tileManager;

    private TextMeshProUGUI winText;
    private bool gameActive = true;
    private int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();

        tileManager.StartSelectingTiles();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Increase score (successfully completed steps) by one.
    /// </summary>
    public void UpdateScore()
    {
        score += 1;
        //scoreText.text = score.ToString();

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
        }
    }
}
