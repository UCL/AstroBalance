using System.Collections;
using TMPro;
using UnityEngine;

public class SpaceWalkingManager : MonoBehaviour
{

    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Complete steps required to win")]
    private int winningScore = 20;
    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;
    [SerializeField, Tooltip("Seconds until first tile activation")]
    private int activationDelay = 1;
    [SerializeField, Tooltip("Direction tile manager")]
    private TileManager tileManager;

    private TextMeshProUGUI winText;
    private bool gameActive = true;
    private int score = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(StartTileActivation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator StartTileActivation()
    {
        yield return new WaitForSeconds(activationDelay);
        tileManager.ActivateNextTile();
    }

    /// <summary>
    /// Increase score (successfully completed steps) by one.
    /// </summary>
    public void UpdateScore()
    {
        score += 1;
        scoreText.text = score.ToString();

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

            winText.text = "Congratulations! \n \n You completed " + score + " steps";
            winScreen.SetActive(true);
        }
    }
}
