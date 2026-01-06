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
        if (useMouseForTracker)
        {
            var mousePos = Input.mousePosition;
            gp.X = mousePos.x;
            gp.Y = mousePos.y;
            gp.TimeStampMicroSeconds = (long)(Time.timeSinceLevelLoad * 1000000);
        }
        else
        {
            gp = tracker.getGazePoint();
        }
        transform.Translate(new Vector3(gp.X, gp.Y, 0f));
        gazeBuffer.Add(gp);
        Debug.Log(gazeBuffer.Size + " : " + gazeBuffer.Get().X);

        // Debug.Log("Rocket control update" + TrackerInterface.getGazePoint()[0]);
    }
}
