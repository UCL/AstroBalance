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
    Tracker tracker;

    [
        SerializeField,
        Tooltip("Set to true to substitute the mouse for the eye tracker (for debugging purposes)")
    ]
    private bool useMouseForTracker = false;

    [SerializeField, Tooltip("The game object that changes size based on head speed.")]
    private ParticleSystem speedObject;

    [SerializeField, Tooltip("The time (in seconds) to launch.")]
    private int launchTime = 30;
    private float timeToLaunch;

    [SerializeField, Tooltip("The capacity of the head pose buffer to use.")]
    private int headPoseBufferCapacity = 10;

    [SerializeField, Tooltip("The time in seconds to measure head speed over.")]
    private float speedTime = 1.0f;

    [SerializeField, Tooltip("A scale factor to control the ratio of head speed to flame size.")]
    private float speedScale = 1.0f;

    [SerializeField, Tooltip("The minimum head speed required to reduce the launch timer.")]
    private float minimumSpeed = 20;

    [SerializeField, Tooltip("Launch acceleration factor. Bigger for faster launch.")]
    private float acceleration = 0.04f;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI speedStatusText;

    // options for the gaze elememts
    [SerializeField, Tooltip("Time between new random numbers in seconds.")]
    private float timerDuration = 1.0F;

    [SerializeField, Tooltip("Sprites to display on the countdown.")]
    private List<Sprite> countDownSprites;

    [SerializeField, Tooltip("The capacity of the gaze buffer to use.")]
    private int gazeBufferCapacity = 100;

    [SerializeField, Tooltip("The time in seconds that the gaze should be steady for.")]
    private float gazeTime = 3.0f;

    [SerializeField, Tooltip("The tolerance in unity coordinates that gaze needs to stay within.")]
    private float gazeTolerance = 3.0f;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI gazeStatusText;

    [SerializeField, Tooltip("The game object the user is supposed to look at.")]
    private GameObject targetObject;

    [
        SerializeField,
        Tooltip("Adaptive difficulty, higher numbers are more difficult"),
        Range(1, 10)
    ]
    private float adaptiveDifficulty;

    [
        SerializeField,
        Tooltip("The maximum number of previous games to retrieve to determine experience based difficulty")
    ]
    private int maxPreviousGames = 100;

    [SerializeField, Tooltip("A text box for the instructions.")]
    private TextMeshProUGUI instructionsText;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    // head speed parameters
    private HeadAngleBuffer headPitchBuffer;
    private HeadAngleBuffer headYawBuffer;
    private bool usePitch; //true if we're using pitch speed, false if we're using yaw speed.
    private RocketLaunchData gameData;
    private float rocketSpeed;
    private int minDataRequired = 2; // we need at least 2 data points to calculate a speed or steadiness

    // gaze steadiness paraemeters
    private float timeToSpriteChange;
    private Sprite countDownSprite = null;
    private GazeBuffer gazeBuffer;
    private Vector3 startScale;

    private string saveFilename = "RocketLaunchScores";

    private TextMeshProUGUI winText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rocketSpeed = 0f;
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        winScreen.SetActive(false);
        tracker = FindFirstObjectByType<Tracker>();

        SaveData<RocketLaunchData> saveData = new(saveFilename);

	launchCode = FindFirstObjectByType<LaunchCode>();
        IEnumerable<RocketLaunchData> lastGameData = saveData.GetLastNGamesData(maxPreviousGames);
        Debug.Log("There are " + lastGameData.Count() + " previous games");

        adaptiveDifficulty *= (maxPreviousGames + lastGameData.Count())/maxPreviousGames;

        if (lastGameData.Count() == 0)
        {
            usePitch = true;
        }
        else
        {
            usePitch = !lastGameData.Last().pitch;
        }
        headPitchBuffer = new HeadAngleBuffer(headPoseBufferCapacity, minDataRequired);
        headYawBuffer = new HeadAngleBuffer(headPoseBufferCapacity, minDataRequired);
        instructionsText.text = usePitch
            ? "Nod your head and repeat the code to launch the rocket!"
            : "Shake your head and repeat the code to launch the rocket!";
        gameData = new RocketLaunchData();
        timeToLaunch = (float)launchTime * adaptiveDifficulty;
        gazeBuffer = new GazeBuffer(gazeBufferCapacity, minDataRequired);
        incrementCountDownCode();
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
            bool gazeIsSteady = false;
            float headSpeed = 0f;

            if (usePitch)
            {
                headSpeed = headPitchBuffer.getSpeed(speedTime) - headYawBuffer.getSpeed(speedTime);
            }
            else
            {
                headSpeed = headYawBuffer.getSpeed(speedTime) - headPitchBuffer.getSpeed(speedTime);
            }
            headSpeed = Mathf.Max(0, headSpeed); // Clamp to zero to avoid negative speeds

            var myEmitter = speedObject.emission;
            myEmitter.rateOverTime = headSpeed * speedScale;

            // gaze steadiness
            float targetX = 0f;
            float targetY = 0f;
            if (targetObject != null)
            {
                // use centre of bounds in case the target object is not centred
                targetX = targetObject.transform.GetComponent<Renderer>().bounds.center.x;
                targetY = targetObject.transform.GetComponent<Renderer>().bounds.center.y;
                Vector2 gazeTol = new Vector2(
                    targetObject.transform.GetComponent<Renderer>().bounds.extents.x,
                    targetObject.transform.GetComponent<Renderer>().bounds.extents.y
                );
                gazeIsSteady = gazeBuffer.gazeSteady(
                    gazeTime,
                    gazeTolerance * gazeTol.magnitude,
                    targetX,
                    targetY
                );
            }
            else
            {
                gazeIsSteady = gazeBuffer.gazeSteady(gazeTime, gazeTolerance);
            }

            writeDebugInformation(headSpeed, gazeItem, targetX, targetY, gazeIsSteady);

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
            headPose.Rotation.YawDegrees = mousePos.x;
            headPose.Rotation.PitchDegrees = mousePos.y;
            headPose.Rotation.RollDegrees = 0f;
            headPose.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);

            gazeItem.gazePoint.X = mousePos.x;
            gazeItem.gazePoint.Y = mousePos.y;
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
        HeadPitchItem headPitch = new HeadPitchItem(headPose);
        HeadYawItem headYaw = new HeadYawItem(headPose);
        headPitchBuffer.addIfNew(headPitch);
        headYawBuffer.addIfNew(headYaw);
        gazeBuffer.addIfNew(gazeItem);

        return gazeItem;
    }

    private void writeDebugInformation(
        float headSpeed,
        GazeItem gazeItem,
        float targetX,
        float targetY,
        bool gazeIsSteady
    )
    {
        if (speedStatusText != null)
        {
            string speedText = usePitch ? "Pitch Speed" : "Yaw Speed";
            speedStatusText.text = speedText + " = " + headSpeed;
        }
        if (gazeStatusText != null)
        {
            string steadyText = gazeIsSteady ? "Gaze is steady" : "Gaze is not steady";
            gazeStatusText.text =
                "Look here -> "
                + targetX
                + ", "
                + targetY
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
        winText.text = "Blast Off! Well Done.";
        winScreen.SetActive(true);
        SaveGameData();
        this.enabled = false;
    }

    private void SaveGameData()
    {
        gameData.gameCompleted = true;
        gameData.pitch = usePitch;
        gameData.launchTimeSeconds = launchTime;
        gameData.LogEndTime();

        SaveData<RocketLaunchData> saveData = new(saveFilename);
        saveData.SaveGameData(gameData);
    }

    private void incrementCountDownCode()
    {
        Sprite newCountDownSprite = countDownSprites[Random.Range(0, countDownSprites.Count)];
        // remove the number from the list to avoid selected a repeat number next time.
        countDownSprites.Remove(newCountDownSprite);
        if (countDownSprite != null)
        {
            countDownSprites.Add(countDownSprite);
        }
        countDownSprite = newCountDownSprite;
        startScale = targetObject.GetComponent<SpriteRenderer>().transform.localScale;
        targetObject.GetComponent<SpriteRenderer>().transform.localScale =
            startScale / adaptiveDifficulty;
        targetObject.GetComponent<SpriteRenderer>().sprite = countDownSprite;
        timeToSpriteChange = timerDuration;
    }
}
