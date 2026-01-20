using UnityEngine;
using TMPro;
using Tobii.GameIntegration.Net;
using TrackerBuffers;


public class rocket_control : MonoBehaviour
{
    Tracker tracker;

    [SerializeField, Tooltip("Set to true to substitute the mouse for the eye tracker (for debugging purposes)")]
    private bool useMouseForTracker = false;

    [SerializeField, Tooltip("The game object the user is supposed to look at.")]
    private GameObject targetObject;

    [SerializeField, Tooltip("The game object that changes size based on head speed.")]
    //private GameObject speedObject;
    private ParticleSystem speedObject;

    [SerializeField, Tooltip("The capacity of the gaze buffer to use.")]
    private int gazeBufferCapacity = 100;

    [SerializeField, Tooltip("The capacity of the head pose buffer to use.")]
    private int headPoseBufferCapacity = 10;

    [SerializeField, Tooltip("The time in seconds that the gaze should be steady for.")]
    private float gazeTime = 3.0f;

    [SerializeField, Tooltip("The tolerance in pixels that gaze needs to stay within.")]
    private float gazeTolerance = 3.0f;

    [SerializeField, Tooltip("The time in seconds to measure head speed over.")]
    private float speedTime = 1.0f;

    [SerializeField, Tooltip("An optional status text window for debugging.")]
    private TextMeshProUGUI statusText;

    private GazeBuffer gazeBuffer;
    private HeadPoseBuffer headPoseBuffer;
    private bool pitch; //true if we're using pitch speed, false if we're using yaw speed.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        gazeBuffer = new GazeBuffer(gazeBufferCapacity);
        headPoseBuffer = new HeadPoseBuffer(headPoseBufferCapacity);
	pitch = PitchOrYaw.GetPitch();
    }

    // Update is called once per frame
    void Update()
    {
        GazePoint gp = new GazePoint();
        HeadPose headPose = new HeadPose();
        if (useMouseForTracker)
        {
            var mousePos = Input.mousePosition;
            gp.X = mousePos.x;
            gp.Y = mousePos.y;
            gp.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
            headPose.Position.X = mousePos.x;
            headPose.Position.Y = 0f;
            headPose.Position.Z = 0.5f;
            headPose.Rotation.YawDegrees = 0f;
            headPose.Rotation.PitchDegrees = 0f;
            headPose.Rotation.RollDegrees = 0f;
            headPose.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
        }
        else
        {
            gp = tracker.getGazePoint();
            Vector2 worldGaze = tracker.ConvertGazePointToWorldCoordinates(gp);
            gp.X = worldGaze.x;
            gp.Y = worldGaze.y;
            headPose = tracker.getHeadPose();
        }
        if (!gazeBuffer.addIfNew(gp))
        {
            Debug.Log("No new gaze point.");
        }
        if (!headPoseBuffer.addIfNew(headPose))
        {
            Debug.Log("No new head pose.");
        }
        bool gazeIsSteady = false;
        GazePoint targetPoint = new GazePoint();
        if (targetObject != null)
        {
            targetPoint.X = targetObject.transform.position.x;
            targetPoint.Y = targetObject.transform.position.y;
            targetPoint.X = 0f;
            targetPoint.Y = 0f;
            gazeIsSteady = gazeBuffer.gazeSteady(gazeTime, gazeTolerance, targetPoint);
        }
        else
        {
            gazeIsSteady = gazeBuffer.gazeSteady(gazeTime, gazeTolerance);
        }

        float headSpeed = headPoseBuffer.getSpeed(speedTime);

	    if (statusText != null)
	    {
	        string speedText = pitch ? "Pitch Speed" : "Yaw Speed";
	        string steadyText = gazeIsSteady ? "Gaze is steady" : "Gaze is not steady";
	        statusText.text = "Look here -> " + targetPoint.X + ", " + targetPoint.Y + "\n" +
			           "Looking here -> " + gp.X + ", " + gp.Y + "\n" +
			           speedText + " = " + headSpeed + "\n" +
			        steadyText;
	    }
        var myEmitter = speedObject.emission;
        if (gazeIsSteady)
        {
            myEmitter.rateOverTime = headSpeed;
        }
        else
        {
            myEmitter.rateOverTime = 0f;
        }
    }
}
