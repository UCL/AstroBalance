using System.Collections.Generic;
using System.Linq;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class SwayLine : MonoBehaviour
{
    [SerializeField, Tooltip("Pose hold timer")]
    private CountdownTimer poseHoldTimer;

    [SerializeField, Tooltip("Head x movement scaling (moving 1mm, moves this many unity units)")]
    private float headXScaling = 0.01f;

    [SerializeField, Tooltip("Head x movement tolerance (mm) - beyond this limit scoring stops")]
    private float headXTolerance = 100;

    [SerializeField, Tooltip("Inside x range colour")]
    private Color inRangeColor = Color.white;

    [SerializeField, Tooltip("Outside x range colour")]
    private Color outRangeColor = Color.black;

    [
        SerializeField,
        Tooltip("Maximum number of head x position readings to keep in the buffer at one time")
    ]
    private int maxNItemsInBuffer = 100;

    [
        SerializeField,
        Tooltip("The minimum number of head position readings needed to calculate a sway velocity")
    ]
    private int minNItemsForVelocity = 5;

    [SerializeField, Tooltip("The number of seconds to calculate sway velocity over")]
    private float samplingIntervalSeconds = 0.5f;

    private Tracker tracker;
    private SpriteRenderer spriteRenderer;
    private ZeroGravityManager gameManager;
    private bool scoringActive = false;
    private float timeIncrement; // time increment required to score
    private float timeOfNextScoreIncrease; // time remaining on pose hold timer at next score increase
    private bool headOutOfRange = false;
    private int nTimesOutOfRange = 0; // number of times the player's head has gone out of range while scoring is active

    private HeadPoseBuffer headPoseBuffer;
    private List<float> swayVelocities = new List<float>(); // recorded sway velocities while scoring is active
    private float timeOfNextVelocity; // time remaining on pose hold timer at next sway velocity measurement
    private bool undetectedInWindow = false; // whether the tracker couldn't detect the player since the last velocity measurement

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindFirstObjectByType<ZeroGravityManager>();
    }

    /// <summary>
    /// Allow scoring when the head is in range (and no scoring when it is out of range).
    /// </summary>
    /// <param name="timeLimit">Total time limit of this pose</param>
    /// <param name="timeIncrement">Time in seconds head must stay in range to score</param>
    public void ActivateScoring(int timeLimit, float timeIncrement)
    {
        nTimesOutOfRange = 0;
        swayVelocities = new List<float>();
        headPoseBuffer = new HeadPoseBuffer(maxNItemsInBuffer, minNItemsForVelocity);
        undetectedInWindow = false;
        this.timeIncrement = timeIncrement;

        // We base scoring on the pose hold timer so that everything stays in sync,
        // and exactly matches the displayed countdown times
        poseHoldTimer.StartCountdown(timeLimit);
        timeOfNextScoreIncrease = timeLimit - timeIncrement;
        timeOfNextVelocity = timeLimit - samplingIntervalSeconds;
        scoringActive = true;
    }

    /// <summary>
    /// Deactivate all scoring. The score will remain the same whether the head
    /// is in range or not.
    /// </summary>
    public void DeactivateScoring()
    {
        // One last call to HandleScoring(), to make sure all relevant hold
        // seconds have been scored
        HandleScoring();
        poseHoldTimer.StopCountdown();
        scoringActive = false;
    }

    /// <summary>
    /// Get the number of times the player's head went out of range while scoring
    /// was active.
    /// This value is reset each time scoring is activated.
    /// </summary>
    public int GetNTimesOutOfRange()
    {
        return nTimesOutOfRange;
    }

    /// <summary>
    /// Return the average sway velocity (in cm per second) while scoring was active.
    /// This value is reset each time scoring is activated.
    /// </summary>
    public float GetMeanSwayVelocity()
    {
        return swayVelocities.Average();
    }

    // Update is called once per frame
    void Update()
    {
        if (tracker.isPlayerDetected())
        {
            spriteRenderer.enabled = true;
            UpdateLinePosition();
        }
        else
        {
            spriteRenderer.enabled = false;

            if (!headOutOfRange && scoringActive)
            {
                nTimesOutOfRange++;
            }
            headOutOfRange = true;
        }

        if (scoringActive)
        {
            UpdateSwayVelocities();
        }
    }

    /// <summary>
    /// Update measurements of sway velocity (head x position velocity)
    /// </summary>
    private void UpdateSwayVelocities()
    {
        // If the player can't be detected by the tracker
        if (!tracker.isPlayerDetected())
        {
            undetectedInWindow = true;
        }
        else
        {
            HeadPose headPose = tracker.getHeadPose();
            headPoseBuffer.addIfNew(new HeadPoseItem(headPose));
        }

        // Every 'samplingIntervalSeconds', record the head x velocity averaged over that time period
        if (poseHoldTimer.GetTimeRemaining() <= timeOfNextVelocity)
        {
            float swayVelocity = headPoseBuffer.getSpeed(samplingIntervalSeconds, HeadPoseAxis.X);

            // If there aren't enough recorded head x positions yet, the returned velocity is zero.
            // We don't want to include these readings in the overall averages.
            if (swayVelocity > 0)
            {
                // Record sway velocity in cm per second
                swayVelocities.Add(swayVelocity / 10);
            }

            undetectedInWindow = false;
            timeOfNextVelocity -= samplingIntervalSeconds;
        }
    }

    private void UpdateLinePosition()
    {
        HeadPose currentHeadPose = tracker.getHeadPose();
        float xPosMm = currentHeadPose.Position.X;
        float rollDegrees = currentHeadPose.Rotation.RollDegrees;

        bool outOfRange = Mathf.Abs(xPosMm) >= headXTolerance;

        if (outOfRange)
        {
            spriteRenderer.color = outRangeColor;

            if (!headOutOfRange && scoringActive)
            {
                nTimesOutOfRange++;
            }
            headOutOfRange = true;
        }
        else
        {
            HandleScoring();
            headOutOfRange = false;
            spriteRenderer.color = inRangeColor;
        }

        // We only move the sway line on the x axis - left/right (we don't care about
        // changes in head height, or distance from screen)
        transform.position = new Vector3(xPosMm * headXScaling, transform.position.y, 0);

        // We rotate only with head roll
        transform.eulerAngles = new Vector3(0, 0, -rollDegrees);
    }

    private void HandleScoring()
    {
        if (!scoringActive)
        {
            return;
        }

        if (headOutOfRange)
        {
            // We've just returned from the head being out of range,
            // so set a new score time goal
            timeOfNextScoreIncrease = poseHoldTimer.GetTimeRemaining() - timeIncrement;
        }
        else
        {
            if (poseHoldTimer.GetTimeRemaining() <= timeOfNextScoreIncrease)
            {
                gameManager.UpdateScore();
                timeOfNextScoreIncrease -= timeIncrement;
            }
        }
    }
}
