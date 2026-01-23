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

    private Tracker tracker;
    private SpriteRenderer spriteRenderer;
    private ZeroGravityManager gameManager;
    private bool scoringActive = false;
    private float timeIncrement; // time increment required to score
    private float timeOfNextScoreIncrease; // time remaining on pose hold timer at next score increase
    private bool headOutOfRange = false;

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
        this.timeIncrement = timeIncrement;
        // We base scoring on the pose hold timer so that everything stays in sync,
        // and exactly matches the displayed countdown times
        poseHoldTimer.StartCountdown(timeLimit);
        timeOfNextScoreIncrease = timeLimit - timeIncrement;
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

    // Update is called once per frame
    void Update()
    {
        HeadPose currentHeadPose = tracker.getHeadPose();
        float xPosMm = currentHeadPose.Position.X;
        float rollDegrees = currentHeadPose.Rotation.RollDegrees;

        bool outOfRange = Mathf.Abs(xPosMm) >= headXTolerance;

        if (outOfRange)
        {
            spriteRenderer.color = outRangeColor;
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
