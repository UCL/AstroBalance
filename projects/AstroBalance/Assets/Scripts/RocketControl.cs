using UnityEngine;
using Tobii.GameIntegration.Net;
using GazeBuffer;


public class rocket_control : MonoBehaviour
{
    Tracker tracker;

    [SerializeField, Tooltip("Set to true to substitute the mouse for the eye tracker (for debugging purposes)")]
    private bool useMouseForTracker = false;

    [SerializeField, Tooltip("The capacity of the gaze buffer to use.")]
    private int gazeBufferCapacity = 100;

    [SerializeField, Tooltip("The time in seconds that the gaze should be steady for.")]
    private float gazeTime = 3.0f;

    [SerializeField, Tooltip("The tolerance in pixels that gaze needs to stay within.")]
    private float gazeTolerance = 3.0f;

    private gazeBuffer gazeBuffer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        gazeBuffer = new gazeBuffer(gazeBufferCapacity, true);
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
	    headPose.Position.X = 0f;
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
	    headPose = tracker.getHeadPose();
        }
        transform.Translate(new Vector3(gp.X, gp.Y, 0f));
        if (!gazeBuffer.addIfNew(gp))
        {
            Debug.Log("No new gaze point.");
        }

        if (gazeBuffer.gazeSteady(gazeTime, gazeTolerance, gp))
        {
            Debug.Log("Gaze is steady");
        }
        else
        {
            Debug.Log("Gaze is not steady");
        }
        // Debug.Log("Rocket control update" + TrackerInterface.getGazePoint()[0]);
    }
}
