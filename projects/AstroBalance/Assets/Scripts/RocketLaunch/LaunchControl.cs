using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;

class DifficultyLevel
{
    public float time;
    public float targetSize;
}

/// <summary>
/// Manages the size of a rocket flame based on head speed, and controls
/// overall game time.
/// </summary>
public class LaunchControl : MonoBehaviour
{
    [SerializeField, Tooltip("The time (in seconds) required to launch.")]
    private float launchTime = 30;

    [Header("Head Movement Variables")]
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

    [
        SerializeField,
        Tooltip("The minimum number of head pose readings used to calculate a head speed")
    ]
    private int minNItemsForSpeed = 5;

    [Header("Steady Gaze Variables")]
    [SerializeField, Tooltip("Time between new random numbers in seconds.")]
    private float timerDuration = 1.0F;

    [SerializeField, Tooltip("The time in seconds that the gaze should be steady for.")]
    private float gazeTime = 3.0f;

    [
        SerializeField,
        Tooltip(
            "The tolerance in unity coordinates that gaze needs to stay within (the targetObject is scaled to match)"
        )
    ]
    private float gazeTolerance = 3.0f;

    [
        SerializeField,
        Tooltip(
            "The necessary scale factor to convert the target screen scale to the intended physical scale."
        )
    ]
    float physicalScaleFactor = 0.04f;

    [SerializeField, Tooltip("The minimum number of gaze points used to calculate steadiness")]
    private int minNItemsForGaze = 5;

    [SerializeField, Tooltip("The game object the user is supposed to look at.")]
    private GameObject targetObject;

    [SerializeField, Tooltip("Rocket object to be launched.")]
    private GameObject Rocket;
    private float rocketSpeed;

    [Header("Adaptive Difficulty Variables")]
    [
        SerializeField,
        Tooltip(
            "The maximum number of previous games to retrieve to determine experience based difficulty"
        )
    ]
    private int maxPreviousGames = 100;
    private int difficultyLevel = 0;

    [
        SerializeField,
        Tooltip("Adaptive difficulty, higher numbers are more difficult"),
        Range(1, 10)
    ]
    private float adaptiveDifficulty;

    [Header("Save data Variables")]
    [SerializeField, Tooltip("The interval between samples for the save data.")]
    private float samplingIntervalSeconds = 0.5f;

    [SerializeField, Tooltip("Whether to write sampled speeds to a file called rocket-speeds.txt")]
    private bool writeSampledSpeeds = false;

    [SerializeField, Tooltip("Launch Code Display Text")]
    private TextMeshProUGUI launchText;
    private int currentCode;

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

    [SerializeField, Tooltip("Task durations for different levels (ordered)")]
    private float[] taskTimes = { 20f, 30f, 45f, 60f, 75f, 90f, 105f, 120f };

    [SerializeField, Tooltip("Target sizes for different levels (ordered)")]
    private float[] targetSizes = { 25f, 22f, 20f, 18f, 16f, 14f, 12f };

    private List<DifficultyLevel> levels;

    private Tracker tracker;
    private float timeToLaunch;

    // head speed parameters
    private HeadPoseBuffer headPoseBuffer;
    private bool usePitch; //true if we're using pitch speed, false if we're using yaw speed.
    private float minimumSpeed; // minimum head speed required for this game
    private float headSpeed; // current head speed
    private RocketLaunchData gameData;
    private float mouseToGazeScale = 10f; // if we're debugging using the mouse the reported speeds are much higher than with gaze.

    // gaze steadiness parameters
    private float timeToCodeChange;
    private GazeBuffer gazeBuffer;

    // Sampling for save data parameters
    private List<float> headSpeedSamples = new List<float>();
    private int nSamplesGazeSteady = 0;
    private int nSamplesGazeNotSteady = 0;
    private float timeToNextSample;

    // track when the player is in or out of range of the tracker
    bool outOfRange = false;
    float secondsSinceLastOutOfRange = 0;

    private string saveFilename = "RocketLaunchScores";
    private bool gameActive = true;

    private TextMeshProUGUI winText;

    [SerializeField, Tooltip("Flag for turning off features in Demo mode")]
    private bool isDemo = false;

    // Awake is called once when the script instance is loaded
    void Awake()
    {
        rocketSpeed = 0f;
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        winScreen.SetActive(false);
        tracker = FindFirstObjectByType<Tracker>();

        IEnumerable<RocketLaunchData> lastGameData;
        if (!isDemo)
        {
            SaveGameData<RocketLaunchData> saveData = new(saveFilename);

            lastGameData = saveData.GetLastNComplete(maxPreviousGames);
        }
        else
        {
            lastGameData = new List<RocketLaunchData>();
        }

        // Adjust the adaptive difficulty (size of gaze target and time to launch) based on
        // how many previous games are in the save games data

        ConstructDifficultyLevels();
        SetDifficultyLevel(lastGameData);

        // Make sure buffers have a large enough capacity to cover sampling for save data + the speed/gaze steady time for gameplay
        float maxSecondsOfPoseInfo = Mathf.Max(new float[] { samplingIntervalSeconds, speedTime });
        float maxSecondsOfGazeInfo = Mathf.Max(new float[] { samplingIntervalSeconds, gazeTime });
        headPoseBuffer = new HeadPoseBuffer(maxSecondsOfPoseInfo, minNItemsForSpeed);
        gazeBuffer = new GazeBuffer(maxSecondsOfGazeInfo, minNItemsForGaze);

        instructionsText.text = usePitch
            ? "Move your head up and down and repeat the code to launch the rocket!"
            : "Move your head side to side and repeat the code to launch the rocket!";
        timeToLaunch = launchTime;
        timeToNextSample = samplingIntervalSeconds;

        timeToCodeChange = timerDuration;
    }

    private void AdjustTargetForDifficulty()
    {
        var difficulty = levels[difficultyLevel];
        Debug.Log(
            "Difficulty level set at "
                + difficultyLevel
                + ", with timer "
                + difficulty.time
                + " and size "
                + difficulty.targetSize
        );
        timeToLaunch = difficulty.time;
        targetObject.transform.localScale =
            difficulty.targetSize * physicalScaleFactor * new Vector3(1, 1, 1);
    }

    private void SetDifficultyLevel(IEnumerable<RocketLaunchData> lastGameData)
    {
        if (lastGameData.Count() == 0 || isDemo)
        {
            usePitch = true;
            difficultyLevel = 0;
        }
        else
        {
            // Difficulty level progresses when it has been completed twice successfuly for both yaw and pitch
            difficultyLevel = lastGameData.Last().difficultyLevel;
            if (checkForDifficultyIncrease(lastGameData))
            {
                difficultyLevel = Math.Min(difficultyLevel + 1, levels.Count);
            }
            if (lastGameData.Last().headMovementPlane == "yaw")
            {
                usePitch = true;
            }
            else
            {
                usePitch = false;
            }
        }
        minimumSpeed = usePitch ? minimumSpeedPitch : minimumSpeedYaw;
    }

    private bool checkForDifficultyIncrease(IEnumerable<RocketLaunchData> lastGameData)
    {
        int[] successful_games = { 0, 0 };
        for (int i = lastGameData.Count() - 1; i >= 0; i--)
        {
            var data = lastGameData.ElementAt(i);
            if (data.difficultyLevel != difficultyLevel)
            {
                break;
            }

            if (data.gameCompleted)
            {
                if (data.headMovementPlane == "yaw")
                {
                    successful_games[1]++;
                }
                else
                {
                    successful_games[0]++;
                }
            }
        }
        return ((successful_games[0] >= 2) & (successful_games[1] >= 2));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameData = new RocketLaunchData();
        AdjustTargetForDifficulty();
        InitialiseTarget();
    }

    private void ConstructDifficultyLevels()
    {
        Debug.Log("Construct difficulty levels");
        levels = new();
        int i = 0,
            j = 0;
        bool inc_time = true;
        while (i < taskTimes.Length & j < targetSizes.Length)
        {
            var l = new DifficultyLevel();
            l.time = taskTimes[i];
            l.targetSize = targetSizes[j];
            if (inc_time)
            {
                i += 1;
                inc_time = false;
            }
            else
            {
                j += 1;
                inc_time = true;
            }
            levels.Add(l);
        }
        Debug.Log("Levels size = " + levels.Count);
    }

    /// <summary>
    /// Initialise sprite of target, and scale size to match gaze tolerance
    /// </summary>
    private void InitialiseTarget()
    {
        targetObject.SetActive(false);
        incrementCountDownCode();
        targetObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // If time limit reached, end game
        if (timeToLaunch <= 0)
        {
            launchText.text = "";
            if (Rocket.transform.position.y < 60)
            {
                rocketSpeed += Time.deltaTime * acceleration;
                Rocket.transform.Translate(Vector3.up * rocketSpeed);
                // Camera trails behind slightly so that the rocket escapes view
                Camera.main.transform.Translate(Vector3.up * (rocketSpeed * 0.5f));
            }
            else
            {
                EndGame();
            }
        }
        else
        {
            GazeItem gazeItem = AddToBuffers();
            SetPlayerIsOutOfRangeFlag();
            SampleForSaveData();

            headSpeed = CalculateHeadSpeed(speedTime, true);
            bool gazeIsSteady = CalculateGazeSteady(gazeTime);

            writeDebugInformation(headSpeed, gazeItem, gazeIsSteady);

            if (timeToCodeChange > 0)
            {
                if (gazeIsSteady)
                {
                    timeToCodeChange -= Time.deltaTime;
                }
            }
            else
            {
                incrementCountDownCode();
                timeToCodeChange = timerDuration;
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

    private void SetPlayerIsOutOfRangeFlag()
    {
        if (!useMouseForTracker && !tracker.isPlayerDetected())
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

    /// <summary>
    /// Take samples for head speed / gaze steady for the save data.
    /// Windows including out of range time are excluded.
    /// </summary>
    private void SampleForSaveData()
    {
        if (timeToNextSample > 0)
        {
            timeToNextSample -= Time.deltaTime;
            return;
        }

        if (!outOfRange && secondsSinceLastOutOfRange >= samplingIntervalSeconds)
        {
            bool gazeSample = CalculateGazeSteady(samplingIntervalSeconds);
            if (gazeSample)
            {
                nSamplesGazeSteady++;

                // Only record head speeds while the player's gaze is on target
                float speedSample = CalculateHeadSpeed(samplingIntervalSeconds, false);
                if (speedSample > 0)
                {
                    headSpeedSamples.Add(speedSample);
                }
            }
            else
            {
                nSamplesGazeNotSteady++;
            }
        }

        timeToNextSample = samplingIntervalSeconds;
    }

    /// <summary>
    /// Calculate the current head speed
    /// </summary>
    /// <param name="timeSeconds">The number of seconds of data to use</param>
    /// <param name="compensateOtherAxis">If True, the speed of movement in the perpendicular axis will be subtracted.
    /// (e.g. if this game is using Pitch, then the returned speed will be headPitchSpeed - headYawSpeed)</param>
    private float CalculateHeadSpeed(float timeSeconds, bool compensateOtherAxis)
    {
        if (outOfRange || secondsSinceLastOutOfRange < timeSeconds)
        {
            return 0;
        }

        HeadPoseAxis axis;
        HeadPoseAxis perpendicularAxis;
        if (usePitch)
        {
            axis = HeadPoseAxis.Pitch;
            perpendicularAxis = HeadPoseAxis.Yaw;
        }
        else
        {
            axis = HeadPoseAxis.Yaw;
            perpendicularAxis = HeadPoseAxis.Pitch;
        }

        float currentSpeed = headPoseBuffer.getSpeed(timeSeconds, axis);
        if (compensateOtherAxis)
        {
            currentSpeed -= headPoseBuffer.getSpeed(timeSeconds, perpendicularAxis);
        }

        return Mathf.Max(0, currentSpeed); // Clamp to zero to avoid negative speeds
    }

    /// <summary>
    /// Calculate whether the gaze is steady
    /// </summary>
    /// <param name="timeSeconds">The number of seconds of data to use</param>
    private bool CalculateGazeSteady(float timeSeconds)
    {
        bool gazeIsSteady = false;
        if (outOfRange || secondsSinceLastOutOfRange < timeSeconds)
        {
            return gazeIsSteady;
        }

        if (targetObject != null)
        {
            Vector2 targetCentre = GetTargetCentre();
            gazeIsSteady = gazeBuffer.gazeSteady(
                timeSeconds,
                gazeTolerance,
                targetCentre.x,
                targetCentre.y
            );
        }
        else
        {
            gazeIsSteady = gazeBuffer.gazeSteady(timeSeconds, gazeTolerance);
        }

        return gazeIsSteady;
    }

    private Vector2 GetTargetCentre()
    {
        float targetX = 0f;
        float targetY = 0f;

        if (targetObject is not null)
        {
            targetX = targetObject.transform.position.x;
            targetY = targetObject.transform.position.y;
        }

        return new Vector2(targetX, targetY);
    }

    /// <summary>
    /// Adds latest tracking data to buffers and returns latest gaze information
    /// </summary>
    private GazeItem AddToBuffers()
    {
        HeadPose headPose = new HeadPose();
        GazePoint gazePoint = new GazePoint();
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
            gazePoint.X = mousePoseWorld.x;
            gazePoint.Y = mousePoseWorld.y;
            gazePoint.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
        }
        else
        {
            headPose = tracker.getHeadPose();
            gazePoint = tracker.getGazePoint();

            Vector2 worldGaze = tracker.ConvertGazePointToWorldCoordinates(gazePoint);
            gazePoint.X = worldGaze.x;
            gazePoint.Y = worldGaze.y;
        }

        headPoseBuffer.addIfNew(new HeadPoseItem(headPose));
        GazeItem gazeItem = new(gazePoint);
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
                + gazeItem.getX()
                + ", "
                + gazeItem.getY()
                + "\n"
                + steadyText;
        }
    }

    private void EndGame()
    {
        if (gameActive)
        {
            gameActive = false;
            Destroy(Rocket);
            winText.text = "Blast Off!\nWell Done.";
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
        if (isDemo)
        {
            return;
        }

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

        if (headSpeedSamples.Count() == 0)
        {
            gameData.headSpeedDegPerSecPeak = 0;
            gameData.headSpeedDegPerSecMean = 0;
            gameData.headSpeedDegPerSecMedian = 0;
            gameData.headSpeedDegPerSecSD = 0;
            gameData.percentTimeAbove40DegPerSec = 0;
            gameData.percentTimeGazeOnTarget = 0;
            gameData.timeInAdaptationWindow1 = 0;
            gameData.timeInAdaptationWindow2 = 0;
            gameData.timeInAdaptationWindow3 = 0;
            gameData.timeInAdaptationWindow4 = 0;
        }
        else
        {
            gameData.headSpeedDegPerSecPeak = MathsUtilities.RoundTo2DecimalPlaces(
                headSpeedSamples.Max()
            );
            gameData.headSpeedDegPerSecMean = MathsUtilities.RoundTo2DecimalPlaces(
                headSpeedSamples.Average()
            );
            float median = MathsUtilities.Median(headSpeedSamples);
            gameData.headSpeedDegPerSecMedian = MathsUtilities.RoundTo2DecimalPlaces(median);
            float standardDeviation = MathsUtilities.StandardDeviation(headSpeedSamples);
            gameData.headSpeedDegPerSecSD = MathsUtilities.RoundTo2DecimalPlaces(standardDeviation);

            int nSamplesAbove40DegPerSec = 0;
            int nSamplesAdaptationWindow1 = 0;
            int nSamplesAdaptationWindow2 = 0;
            int nSamplesAdaptationWindow3 = 0;
            int nSamplesAdaptationWindow4 = 0;

            foreach (float headSpeed in headSpeedSamples)
            {
                if (headSpeed > 40)
                {
                    nSamplesAbove40DegPerSec++;
                }

                if (headSpeed >= 60 && headSpeed < 90)
                {
                    nSamplesAdaptationWindow1++;
                }
                else if (headSpeed >= 90 && headSpeed < 130)
                {
                    nSamplesAdaptationWindow2++;
                }
                else if (headSpeed >= 130 && headSpeed < 180)
                {
                    nSamplesAdaptationWindow3++;
                }
                else if (headSpeed >= 180)
                {
                    nSamplesAdaptationWindow4++;
                }
            }

            float percentTimeAbove40DegPerSec =
                ((float)nSamplesAbove40DegPerSec / headSpeedSamples.Count()) * 100;
            gameData.percentTimeAbove40DegPerSec = MathsUtilities.RoundTo2DecimalPlaces(
                percentTimeAbove40DegPerSec
            );
            float percentTimeGazeOnTarget =
                ((float)nSamplesGazeSteady / (nSamplesGazeSteady + nSamplesGazeNotSteady)) * 100;
            gameData.percentTimeGazeOnTarget = MathsUtilities.RoundTo2DecimalPlaces(
                percentTimeGazeOnTarget
            );

            gameData.timeInAdaptationWindow1 = MathsUtilities.RoundTo2DecimalPlaces(
                nSamplesAdaptationWindow1 * samplingIntervalSeconds
            );
            gameData.timeInAdaptationWindow2 = MathsUtilities.RoundTo2DecimalPlaces(
                nSamplesAdaptationWindow2 * samplingIntervalSeconds
            );
            gameData.timeInAdaptationWindow3 = MathsUtilities.RoundTo2DecimalPlaces(
                nSamplesAdaptationWindow3 * samplingIntervalSeconds
            );
            gameData.timeInAdaptationWindow4 = MathsUtilities.RoundTo2DecimalPlaces(
                nSamplesAdaptationWindow4 * samplingIntervalSeconds
            );
        }

        SaveGameData<RocketLaunchData> saveData = new(saveFilename);
        gameData.sessionNumber = CaptureSessionData.CurrentSessionNumber();
        gameData.gameNumber = saveData.GetNextGameNumber();
        saveData.Save(gameData);

        if (writeSampledSpeeds)
        {
            string filePath = Path.Combine(Application.persistentDataPath, "rocket-speeds.txt");
            IEnumerable<string> lines = headSpeedSamples.Select(v => v.ToString());
            File.WriteAllLines(filePath, lines);
        }

        // Update save data for this session
        if (gameComplete)
        {
            CaptureSessionData.MarkGameAsComplete("nCompleteRocketLaunchGames");
        }
    }

    private void incrementCountDownCode()
    {
        int N = currentCode;
        do
        {
            N = UnityEngine.Random.Range(0, 10);
        } while (N == currentCode);
        currentCode = N;
        launchText.text = N.ToString();
    }
}
