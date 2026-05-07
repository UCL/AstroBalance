using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private int overallScore = 0;
    private int currentPoseScore = 0;
    private bool gameActive = true;
    private ActiveTimer activeTimer = ActiveTimer.None;
    private List<ZeroGravityData> gameData = new List<ZeroGravityData>(); // Each item is data on a single pose
    private string gameStartTime; // the overall game start time, in the format needed for the save data
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

        // Record game start time, so it can be used in all trial save data
        StarMapData data = new();
        data.LogEndTime();
        gameStartTime = data.startTime;

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

        if (poseAvatar.GetCurrentSpriteIndex() >= 0)
        {
            CreateCompletedPoseSaveData();
        }
        currentPoseScore = 0;

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
        overallScore += scorePerTime;
        currentPoseScore += scorePerTime;
        scoreText.text = overallScore.ToString();
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

            winText.text = "Congratulations! \n \n You scored " + overallScore + " points";
            winScreen.SetActive(true);
            SaveGameData(true);
        }
    }

    private void OnDestroy()
    {
        // If the scene is exited early (e.g. with the exit button), then save this
        // partial game's data
        if (gameActive)
        {
            SaveGameData(false);
        }
    }

    private int GetNextPoseNumber()
    {
        return gameData.Count() == 0 ? 1 : gameData.Last().poseNumber + 1;
    }

    private void CreateCompletedPoseSaveData()
    {
        ZeroGravityData poseData = new ZeroGravityData();
        poseData.poseNumber = GetNextPoseNumber();
        poseData.poseType = poseAvatar.GetCurrentSpriteName();
        poseData.poseTimeLimitSeconds = poseHoldSeconds;
        poseData.poseDurationSeconds = poseHoldSeconds;
        poseData.balanceStabilityScore = currentPoseScore;
        gameData.Add(poseData);
    }

    private void SaveGameData(bool gameComplete)
    {
        if (gameData.Count() == 0)
        {
            return;
        }

        // Log end time on first item - this end time will be copied to all
        // trial data
        gameData.ElementAt(0).LogEndTime();
        string endTime = gameData.ElementAt(0).endTime;

        SaveGameData<ZeroGravityData> saveData = new(saveFilename);
        int sessionNumber = CaptureSessionData.CurrentSessionNumber();
        int gameNumber = saveData.GetNextGameNumber();

        // Populate data that is common across all poses
        foreach (ZeroGravityData poseData in gameData)
        {
            poseData.sessionNumber = sessionNumber;
            poseData.gameNumber = gameNumber;
            poseData.startTime = gameStartTime;
            poseData.endTime = endTime;
            poseData.gameCompleted = gameComplete;
        }
        saveData.Save(gameData);

        // Update save data for this session
        if (gameComplete)
        {
            CaptureSessionData.MarkGameAsComplete("nCompleteZeroGravityGames");
        }
    }
}
