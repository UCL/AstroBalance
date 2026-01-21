using TMPro;
using Tobii.GameIntegration.Net;
using TrackerBuffers;
using UnityEngine;
using UnityEngine.UIElements;

public class rocket_control : MonoBehaviour
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

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI statusText;

    [SerializeField, Tooltip("A test box for the instructions.")]
    private TextMeshProUGUI instructionsText;

    [SerializeField, Tooltip("Countdown timer prefab")]
    private CountdownTimer timer;

    [SerializeField, Tooltip("Screen shown upon winning the game")]
    private GameObject winScreen;

    private TextMeshProUGUI winText;
    private HeadPoseBuffer headPoseBuffer;
    private bool usePitch; //true if we're using pitch speed, false if we're using yaw speed.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winText = winScreen.GetComponentInChildren<TextMeshProUGUI>();
        winScreen.SetActive(false);
        tracker = FindFirstObjectByType<Tracker>();
        headPoseBuffer = new HeadPoseBuffer(headPoseBufferCapacity);
        usePitch = PitchOrYaw.GetPitch();
        instructionsText.text = usePitch ? "Nod your head and repeat the code to launch the rocket!" : "Shake your head and repeat the code to launch the rocket!";
        timer.StartCountdown(launchTime);
    }

    // Update is called once per frame
    void Update()
    {
        // If time limit reached, end game
        if (timer.GetTimeRemaining() <= 0)
        {
            EndGame();
        }
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

        headPoseBuffer.addIfNew(headPose);

        float headSpeed = headPoseBuffer.getSpeed(speedTime, usePitch);

        if (statusText != null)
        {
            string speedText = usePitch ? "Pitch Speed" : "Yaw Speed";
            statusText.text = speedText + " = " + headSpeed;
        }
        var myEmitter = speedObject.emission;
        myEmitter.rateOverTime = headSpeed;
    }

    private void EndGame()
    {
        winText.text = "Blast Off! Well Done.";
        winScreen.SetActive(true);
        this.enabled = false;
    }
}
