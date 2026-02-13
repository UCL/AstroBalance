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

    [SerializeField, Tooltip("The capacity of the head pose buffer to use.")]
    private int headPoseBufferCapacity = 10;

    [SerializeField, Tooltip("The time in seconds to measure head speed over.")]
    private float speedTime = 1.0f;

    [SerializeField, Tooltip("A scale factor to control the ratio of head speed to flame size.")]
    private float speedScale = 1.0f;

    [SerializeField, Tooltip("Launch acceleration factor. Bigger for faster launch.")]
    private float acceleration = 0.04f;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI statusText;

    [SerializeField, Tooltip("A test box for the instructions.")]
    private TextMeshProUGUI instructionsText;

    [SerializeField, Tooltip("Countdown timer prefab")]
    private CountdownTimer timer;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    private TextMeshProUGUI winText;
    private HeadAngleBuffer headPitchBuffer;
    private HeadAngleBuffer headYawBuffer;
    private bool usePitch; //true if we're using pitch speed, false if we're using yaw speed.
    private RocketLaunchData gameData;
    private float rocketSpeed;
    private int minDataRequired = 2; // we need at least 2 data points to calculate a speed.
    private string saveFilename = "RocketLaunchScores";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rocketSpeed = 0f;
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        winScreen.SetActive(false);
        tracker = FindFirstObjectByType<Tracker>();

        SaveData<RocketLaunchData> saveData = new(saveFilename);
        RocketLaunchData lastGameData = saveData.GetLastCompleteGameData();

        if (lastGameData == null)
        {
            usePitch = true;
        }
        else
        {
            usePitch = !lastGameData.pitch;
        }
        headPitchBuffer = new HeadAngleBuffer(headPoseBufferCapacity, minDataRequired);
        headYawBuffer = new HeadAngleBuffer(headPoseBufferCapacity, minDataRequired);
        instructionsText.text = usePitch
            ? "Nod your head and repeat the code to launch the rocket!"
            : "Shake your head and repeat the code to launch the rocket!";
        gameData = new RocketLaunchData();
        timer.StartCountdown(launchTime);
    }

    // Update is called once per frame
    void Update()
    {
        // If time limit reached, end game
        if (timer.GetTimeRemaining() <= 0)
        {
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
            HeadPose headPose = new HeadPose();
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
            }
            else
            {
                headPose = tracker.getHeadPose();
            }
            HeadPitchItem headPitch = new HeadPitchItem(headPose);
            HeadYawItem headYaw = new HeadYawItem(headPose);
            headPitchBuffer.addIfNew(headPitch);
            headYawBuffer.addIfNew(headYaw);
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

            if (statusText != null)
            {
                string speedText = usePitch ? "Pitch Speed" : "Yaw Speed";
                statusText.text = speedText + " = " + headSpeed;
            }
            var myEmitter = speedObject.emission;
            myEmitter.rateOverTime = headSpeed * speedScale;
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
}
