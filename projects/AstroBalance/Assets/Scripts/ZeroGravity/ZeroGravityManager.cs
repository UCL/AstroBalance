using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ZeroGravityManager : MonoBehaviour
{
    [SerializeField, Tooltip("Text mesh pro object for score text")]
    private TextMeshProUGUI scoreText;
    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;
    [SerializeField, Tooltip("Countdown timer prefab")]
    private CountdownTimer timer;
    [SerializeField, Tooltip("Avatar showing poses for player to copy")]
    private PoseAvatar poseAvatar;
    [SerializeField, Tooltip("Number of seconds the player must hold each pose")]
    private int poseSeconds = 20;

    private TextMeshProUGUI winText;
    private int score = 0;
    private bool gameActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        poseAvatar.ShowNextSprite();
        timer.StartCountdown(poseSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.GetTimeRemaining() <= 0)
        {
            bool poseAvailable = poseAvatar.ShowNextSprite();
            if (poseAvailable)
            {
                timer.StartCountdown(poseSeconds);
            } else
            {
                EndGame();
            }
        }
    }

    /// <summary>
    /// Increase score (successfully guessed sequences) by one.
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

            winText.text = "Congratulations! \n \n You scored " + score + " points";
            winScreen.SetActive(true);
        }
    }
}
