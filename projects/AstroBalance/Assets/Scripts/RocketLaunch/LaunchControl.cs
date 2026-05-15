using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;

/// <summary>
/// Manages the size of a rocket flame based on head speed, and controls
/// overall game time.
/// </summary>
public class LaunchControl : MonoBehaviour
{
    [SerializeField, Tooltip("The time (in seconds) required to launch.")]
    private float launchTime = 30;

    [Header("Head Movement Variables")]
    [SerializeField, Tooltip("The capacity of the head pose buffer to use.")]
    private int headPoseBufferCapacity = 100;

    [SerializeField, Tooltip("The time in seconds to measure head speed over.")]
    private float speedTime = 2.0f;

    [SerializeField, Tooltip("The minimum head pitch speed required to reduce the launch timer.")]
    private float minimumSpeedPitch = 20;

    [
        SerializeField,
        Tooltip(
            "The minimum head yaw speed required to reduce the launch timer. Usually set higher than pitch, as I can shake my head faster than I can nod"
        )
    ]
    private float minimumSpeedYaw = 40;

    [Header("Steady Gaze Variables")]
    [SerializeField, Tooltip("Time between new random numbers in seconds.")]
    private float timerDuration = 1.0F;

    [SerializeField, Tooltip("The capacity of the gaze buffer to use.")]
    private int gazeBufferCapacity = 100;

    [SerializeField, Tooltip("The time in seconds that the gaze should be steady for.")]
    private float gazeTime = 3.0f;

    [
        SerializeField,
        Tooltip(
            "The tolerance in unity coordinates that gaze needs to stay within (the targetObject is scaled to match)"
        )
    ]
    private float gazeTolerance = 3.0f;

    [SerializeField, Tooltip("The game object the user is supposed to look at.")]
    private GameObject targetObject;

    [Header("Adaptive Difficulty Variables")]
    [
        SerializeField,
        Tooltip(
            "The maximum number of previous games to retrieve to determine experience based difficulty"
        )
    ]
    private int maxPreviousGames = 100;

    [
        SerializeField,
        Tooltip("Adaptive difficulty, higher numbers are more difficult"),
        Range(1, 10)
    ]
    private float adaptiveDifficulty;

    [Header("User Interface Items")]
    [SerializeField, Tooltip("Sprites to display on the countdown.")]
    private List<Sprite> countDownSprites;

    [SerializeField, Tooltip("A text box for the instructions.")]
    private TextMeshProUGUI instructionsText;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    [Header("Launch Speed Variables")]
    [SerializeField, Tooltip("Launch acceleration factor. Bigger for faster launch.")]
    private float acceleration = 0.04f;

    [Header("Debugging Variables")]
    [
        SerializeField,
        Tooltip("Set to true to substitute the mouse for the eye tracker (for debugging purposes)")
    ]
    private bool useMouseForTracker = false;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI gazeStatusText;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI speedStatusText;

    private Tracker tracker;
    private float timeToLaunch;

    // head speed parameters
    private HeadPoseBuffer headPoseBuffer;
    private bool usePitch; //true if we're using pitch speed, false if we're using yaw speed.
    private float minimumSpeed; // minimum head speed required for this game
    private float headSpeed; // current head speed
    private RocketLaunchData gameData;
    private float rocketSpeed;
    private int minDataRequired = 2; // we need at least 2 data points to calculate a speed or steadiness
    private float mouseToGazeScale = 10f; // if we're debugging using the mouse the reported speeds are much higher than with gaze.
    bool outOfRange = false; // whether the player is out of range of the tracker
    float secondsSinceLastOutOfRange = 0; // number of seconds elapsed since the player was last out of range

    // gaze steadiness paraemeters
    private float timeToSpriteChange;
    private Sprite countDownSprite = null;
    private GazeBuffer gazeBuffer;

    private string saveFilename = "RocketLaunchScores";
    private bool gameActive = true;

    private TextMeshProUGUI winText;

    // Awake is called once when the script instance is loaded
    void Awake()
    {
        rocketSpeed = 0f;
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        winScreen.SetActive(false);
        tracker = FindFirstObjectByType<Tracker>();

        SaveGameData<RocketLaunchData> saveData = new(saveFilename);

        IEnumerable<RocketLaunchData> lastGameData = saveData.GetLastNComplete(maxPreviousGames);

        // Adjust the adaptive difficulty (size of gaze target and time to launch) based on
        // how many previous games are in the save games data

        adaptiveDifficulty *=
            ((float)maxPreviousGames + (float)lastGameData.Count()) / (float)maxPreviousGames;
        gazeTolerance /= adaptiveDifficulty;
        launchTime *= adaptiveDifficulty;

        if (lastGameData.Count() == 0 || lastGameData.Last().headMovementPlane == "yaw")
        {
            usePitch = true;
            minimumSpeed = minimumSpeedPitch;
        }
        else
        {
            usePitch = false;
            minimumSpeed = minimumSpeedYaw;
        }
        minimumSpeed = usePitch ? minimumSpeedPitch : minimumSpeedYaw;

        InitialiseTarget();

        headPoseBuffer = new HeadPoseBuffer(headPoseBufferCapacity, minDataRequired);
        instructionsText.text = usePitch
            ? "Nod your head and repeat the code to launch the rocket!"
            : "Shake your head and repeat the code to launch the rocket!";
        timeToLaunch = launchTime;
        gazeBuffer = new GazeBuffer(gazeBufferCapacity, minDataRequired);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameData = new RocketLaunchData();
    }

    /// <summary>
    /// Initialise sprite of target, and scale size to match gaze tolerance
    /// </summary>
    private void InitialiseTarget()
    {
        targetObject.SetActive(false);
        incrementCountDownCode();

        // Match width and height of target to gaze tolerance
        Renderer targetRenderer = targetObject.transform.GetComponent<Renderer>();
        float targetObjectWidth = targetRenderer.bounds.extents.x;
        float targetObjectHeight = targetRenderer.bounds.extents.y;
        Vector3 targetScale = targetRenderer.transform.localScale;
        targetScale.Scale(
            new Vector3(gazeTolerance / targetObjectWidth, gazeTolerance / targetObjectWidth, 1)
        );
        targetRenderer.transform.localScale = targetScale;

        targetObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If time limit reached, end game
        if (timeToLaunch <= 0)
        {
            targetObject.GetComponent<SpriteRenderer>().enabled = false;
            if (transform.position.y < 10)
            {
                rocketSpeed += Time.deltaTime * acceleration;
                transform.Translate(Vector3.up * rocketSpeed);
            }
            else
            {
                EndGame();
            }
        }
        else
        {
            GazeItem gazeItem = AddToBuffers();

            CheckIfPlayerIsOutOfRange();
            CalculateHeadSpeed();
            bool gazeIsSteady = CalculateGazeSteady();

            writeDebugInformation(headSpeed, gazeItem, gazeIsSteady);

            if (timeToSpriteChange > 0)
            {
                if (gazeIsSteady)
                {
                    timeToSpriteChange -= Time.deltaTime;
                }
            }
            else
            {
                incrementCountDownCode();
            }
            if (gazeIsSteady && headSpeed > minimumSpeed)
            {
                timeToLaunch -= Time.deltaTime;
            }
        }
    }

    /// <sumary>
    /// Returns the percentage progress to launch
    /// </summary>
    public float GetProgress()
    {
        return ((launchTime - timeToLaunch) / launchTime) * 100;
    }

    public float HeadSpeed
    {
        get => headSpeed;
    }

    public GameObject TargetObject
    {
        get => targetObject;
    }

    private void CheckIfPlayerIsOutOfRange()
    {
        if (!tracker.isPlayerDetected())
        {
            outOfRange = true;
            secondsSinceLastOutOfRange = 0;
        }
        else
        {
            // If they are already detected as in-range, increase the timer
            if (!outOfRange)
            {
                secondsSinceLastOutOfRange += Time.deltaTime;
            }
            outOfRange = false;
        }
    }

    private void CalculateHeadSpeed()
    {
        if (outOfRange || secondsSinceLastOutOfRange < speedTime)
        {
            headSpeed = 0;
            return;
        }

        if (usePitch)
        {
            headSpeed =
                headPoseBuffer.getSpeed(speedTime, HeadPoseAxis.Pitch)
                - headPoseBuffer.getSpeed(speedTime, HeadPoseAxis.Yaw);
        }
        else
        {
            headSpeed =
                headPoseBuffer.getSpeed(speedTime, HeadPoseAxis.Yaw)
                - headPoseBuffer.getSpeed(speedTime, HeadPoseAxis.Pitch);
        }
        headSpeed = Mathf.Max(0, headSpeed); // Clamp to zero to avoid negative speeds
    }

    private bool CalculateGazeSteady()
    {
        bool gazeIsSteady = false;
        if (outOfRange || secondsSinceLastOutOfRange < gazeTime)
        {
            return gazeIsSteady;
        }

        if (targetObject != null)
        {
            Vector2 targetCentre = GetTargetCentre();
            gazeIsSteady = gazeBuffer.gazeSteady(
                gazeTime,
                gazeTolerance,
                targetCentre.x,
                targetCentre.y
            );
        }
        else
        {
            gazeIsSteady = gazeBuffer.gazeSteady(gazeTime, gazeTolerance);
        }

        return gazeIsSteady;
    }

    private Vector2 GetTargetCentre()
    {
        float targetX = 0f;
        float targetY = 0f;

        if (targetObject is not null)
        {
            // use centre of bounds in case the target object is not centred
            targetX = targetObject.transform.GetComponent<Renderer>().bounds.center.x;
            targetY = targetObject.transform.GetComponent<Renderer>().bounds.center.y;
        }

        return new Vector2(targetX, targetY);
    }

    /// <summary>
    /// Adds latest tracking data to buffers and returns latest gaze information
    /// </summary>
    private GazeItem AddToBuffers()
    {
        HeadPose headPose = new HeadPose();
        GazeItem gazeItem = new GazeItem();
        if (useMouseForTracker)
        {
            var mousePos = Input.mousePosition;
            headPose.Position.X = mousePos.x;
            headPose.Position.Y = 0f;
            headPose.Position.Z = 0.5f;
            headPose.Rotation.YawDegrees = mousePos.x / mouseToGazeScale;
            headPose.Rotation.PitchDegrees = mousePos.y / mouseToGazeScale;
            headPose.Rotation.RollDegrees = 0f;
            headPose.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);

            Vector3 mousePoseWorld = Camera.main.ScreenToWorldPoint(mousePos);
            gazeItem.gazePoint.X = mousePoseWorld.x;
            gazeItem.gazePoint.Y = mousePoseWorld.y;
            gazeItem.gazePoint.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
        }
        else
        {
            headPose = tracker.getHeadPose();

            gazeItem.gazePoint = tracker.getGazePoint();
            Vector2 worldGaze = tracker.ConvertGazePointToWorldCoordinates(gazeItem.gazePoint);
            gazeItem.gazePoint.X = worldGaze.x;
            gazeItem.gazePoint.Y = worldGaze.y;
        }

        headPoseBuffer.addIfNew(new HeadPoseItem(headPose));
        gazeBuffer.addIfNew(gazeItem);

        return gazeItem;
    }

    private void writeDebugInformation(float headSpeed, GazeItem gazeItem, bool gazeIsSteady)
    {
        if (speedStatusText != null)
        {
            string speedText = usePitch ? "Pitch Speed" : "Yaw Speed";
            speedStatusText.text = speedText + " = " + headSpeed;
        }
        if (gazeStatusText != null)
        {
            string steadyText = gazeIsSteady ? "Gaze is steady" : "Gaze is not steady";
            Vector2 targetCentre = GetTargetCentre();

            gazeStatusText.text =
                "Look here -> "
                + targetCentre.x
                + ", "
                + targetCentre.y
                + "\n"
                + "Looking here -> "
                + gazeItem.gazePoint.X
                + ", "
                + gazeItem.gazePoint.Y
                + "\n"
                + steadyText;
        }
    }

    private void EndGame()
    {
        if (gameActive)
        {
            gameActive = false;
            winText.text = "Blast Off! Well Done.";
            winScreen.SetActive(true);
            SaveGameData(true);
            this.enabled = false;
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

    private void SaveGameData(bool gameComplete)
    {
        // Update save data for this game
        gameData.gameCompleted = gameComplete;
        if (usePitch)
        {
            gameData.headMovementPlane = "pitch";
        }
        else
        {
            gameData.headMovementPlane = "yaw";
        }

        gameData.launchTimeSeconds = MathsUtilities.RoundTo2DecimalPlaces(launchTime);
        gameData.gazeTolerance = MathsUtilities.RoundTo2DecimalPlaces(gazeTolerance);
        gameData.minimumHeadSpeed = MathsUtilities.RoundTo2DecimalPlaces(minimumSpeed);
        gameData.LogEndTime();

        // There's no overall timer for this game, so we instead use the logged start / end
        // time (HH:mm:ss) to estimate the game duration.
        TimeSpan gameDuration = DateTime
            .Parse(gameData.endTime)
            .Subtract(DateTime.Parse(gameData.startTime));
        gameData.gameDurationSeconds = MathsUtilities.RoundToNearestInt(
            (float)gameDuration.TotalSeconds
        );

        SaveGameData<RocketLaunchData> saveData = new(saveFilename);
        gameData.sessionNumber = CaptureSessionData.CurrentSessionNumber();
        gameData.gameNumber = saveData.GetNextGameNumber();
        saveData.Save(gameData);

        // Update save data for this session
        if (gameComplete)
        {
            CaptureSessionData.MarkGameAsComplete("nCompleteRocketLaunchGames");
        }
    }

    private void incrementCountDownCode()
    {
        Sprite newCountDownSprite = countDownSprites[
            UnityEngine.Random.Range(0, countDownSprites.Count)
        ];
        // remove the number from the list to avoid selected a repeat number next time.
        countDownSprites.Remove(newCountDownSprite);
        if (countDownSprite != null)
        {
            countDownSprites.Add(countDownSprite);
        }
        countDownSprite = newCountDownSprite;
        targetObject.GetComponent<SpriteRenderer>().sprite = countDownSprite;
        timeToSpriteChange = timerDuration;
    }
}
