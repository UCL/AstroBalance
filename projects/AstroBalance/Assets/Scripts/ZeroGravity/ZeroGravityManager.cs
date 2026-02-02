using System.Collections;
using TMPro;
using UnityEngine;

public class ZeroGravityManager : MonoBehaviour
{
    [SerializeField, Tooltip("Score game object")]
    private GameObject scoreDisplay;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    [SerializeField, Tooltip("Pose hold timer")]
    private CountdownTimer poseHoldTimer;

    [SerializeField, Tooltip("Pose countdown timer")]
    private CountdownTimer poseCountdownTimer;

    [SerializeField, Tooltip("Sway line game object")]
    private SwayLine swayLine;

    [SerializeField, Tooltip("Avatar showing poses for player to copy")]
    private PoseAvatar poseAvatar;

    [SerializeField, Tooltip("Number of seconds to demonstrate each pose")]
    private int poseDisplaySeconds = 2;

    [SerializeField, Tooltip("Number of seconds of countdown to copy pose")]
    private int poseCountdownSeconds = 3;

    [SerializeField, Tooltip("Number of seconds the player must hold each pose")]
    private int poseHoldSeconds = 20;

    [SerializeField, Tooltip("Score per time increment of holding the pose")]
    private int scorePerTime = 5;

    [SerializeField, Tooltip("Number of seconds the pose must be held for a score increase")]
    private int holdTimeIncrement = 1;

    private TextMeshProUGUI winText;
    private TextMeshProUGUI scoreText;
    private int score = 0;
    private bool gameActive = true;
    private ActiveTimer activeTimer = ActiveTimer.None;
    private ZeroGravityData gameData;
    private string saveFilename = "ZeroGravityScores";

    /// <summary>
    /// Keep track of which timers are currently active, and
    /// should be responded to in Update()
    /// </summary>
    private enum ActiveTimer
    {
        None,
        PoseCountdown,
        PoseHold,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        scoreText = scoreDisplay.GetComponentInChildren<TextMeshProUGUI>();

        gameData = new ZeroGravityData();
        StartCoroutine(DisplayNextPose());
    }

    // Update is called once per frame
    void Update()
    {
        if (activeTimer == ActiveTimer.PoseCountdown && poseCountdownTimer.GetTimeRemaining() <= 0)
        {
            HoldPose();
        }
        else if (activeTimer == ActiveTimer.PoseHold && poseHoldTimer.GetTimeRemaining() <= 0)
        {
            StartCoroutine(DisplayNextPose());
        }
    }

    /// <summary>
    /// Display the next pose in the sequence, and start countdown to pose hold.
    /// </summary>
    private IEnumerator DisplayNextPose()
    {
        swayLine.DeactivateScoring();
        activeTimer = ActiveTimer.None;

        poseHoldTimer.gameObject.SetActive(false);
        poseCountdownTimer.gameObject.SetActive(false);
        scoreDisplay.SetActive(false);

        bool poseAvailable = poseAvatar.ShowNextSprite();
        if (!poseAvailable)
        {
            EndGame();
        }

        yield return new WaitForSeconds(poseDisplaySeconds);
        poseCountdownTimer.gameObject.SetActive(true);
        poseCountdownTimer.StartCountdown(poseCountdownSeconds);
        activeTimer = ActiveTimer.PoseCountdown;
    }

    /// <summary>
    /// Activate the hold pose timer and allow scoring when head is in range.
    /// </summary>
    private void HoldPose()
    {
        activeTimer = ActiveTimer.None;

        poseCountdownTimer.gameObject.SetActive(false);
        poseHoldTimer.gameObject.SetActive(true);
        scoreDisplay.gameObject.SetActive(true);
        poseAvatar.HideExplanationText();

        swayLine.ActivateScoring(poseHoldSeconds, holdTimeIncrement);
        activeTimer = ActiveTimer.PoseHold;
    }

    /// <summary>
    /// Increase score - the pose has been held for the time increment.
    /// </summary>
    public void UpdateScore()
    {
        score += scorePerTime;
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
            SaveGameData();
        }
    }

    private void SaveGameData()
    {
        gameData.gameCompleted = true;
        gameData.score = score;
        gameData.LogEndTime();

        SaveData<ZeroGravityData> saveData = new(saveFilename);
        saveData.SaveGameData(gameData);
    }
}
