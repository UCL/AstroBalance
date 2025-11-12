
using UnityEngine;
using UnityEngine.InputSystem;
using Tobii.GameIntegration.Net;
using System.Collections.Generic;

// the plan is to have a single trackerinterface that each game element or minigame can interact with to 
// get tracker information. We'll attach this to the debug text box, then each game object can query it.

public class TrackerInterface : MonoBehaviour
{
    bool usingTobii = false;

    private GazePoint gazePoint = new GazePoint();

    // a public method to expose the gaze point coordinates as a list of floats.
    public List<float> getGazePoint()
    {
        List<float> coords = new List<float>();
        coords.Add(gazePoint.X);
        coords.Add(gazePoint.Y);
        return coords;
    } 

    // We'll need this if we manage to get TrackWindow working. 
    // [System.Runtime.InteropServices.DllImport("user32.dll")]
    // internal static extern System.IntPtr GetActiveWindow();

    void Start()
    {
        gazePoint.X = 0.0F;
        gazePoint.Y = 0.0F;
        gazePoint.TimeStampMicroSeconds = 0;

        Debug.Log("Hello World");
#if UNITY_STANDALONE_WIN
        usingTobii = true;
        TobiiGameIntegrationApi.SetApplicationName("AstroBalance Rocket Launch");

        if (TobiiGameIntegrationApi.IsApiInitialized())
        {
            Debug.Log("Tobii API is initialised");
        }
        else
        {
            Debug.Log("Tobii API is not initialised");
        }
        
        TrackerInfo trackerInfo = TobiiGameIntegrationApi.GetTrackerInfo();
        string trackerUrl = trackerInfo.Url;
         if (trackerInfo.Url != "")
         {
            Debug.Log("Found tracker: " + trackerInfo.FriendlyName + " at " + trackerInfo.Url);
            Debug.Log(" Is it attached ? " + trackerInfo.IsAttached);
         }
         else
         {
             Debug.Log("No tracker found, will try default url");
             // this is a default url, that I got from using the Tobii API sample app.
             trackerUrl =  "tobii-prp://IS5FF-100214127894";
         }

        // the trackerTracker call seems to be what starts the tracking.
        Debug.Log(" Track Tracker = " + TobiiGameIntegrationApi.TrackTracker(trackerUrl));

        // We can set the tracker to track a nominal rectangle. This should be OK for full screen apps.
        TobiiRectangle rectangle = new TobiiRectangle();
        rectangle.Left = 0;
        rectangle.Top = 0;
        rectangle.Right = Screen.currentResolution.width;
        rectangle.Bottom = Screen.currentResolution.height;
        Debug.Log("Track Rectangle = " + TobiiGameIntegrationApi.TrackRectangle(rectangle));

        // In theory we can set to a window, but calling this seems to break the tracking. 
        // Debug.Log("Tracker Window = " + TobiiGameIntegrationApi.TrackWindow(GetActiveWindow()));

#endif
    }

    void OnDisable()
    {
        if (usingTobii)
            TobiiGameIntegrationApi.StopTracking();
    }

    void Update()
    {
        var mouse = Mouse.current;
        // So we can quit the game. This has not impact when running in the editor.
        // https://docs.unity3d.com/ScriptReference/Application.Quit.html
        if (Input.GetKey("escape"))
        {
            Debug.Log("Escape pressed - quitting");
            Application.Quit();
        }

        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Debug.Log("Mouse pressed");
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                Debug.Log("Mouse released");
            }
        }

        if (usingTobii)
        {
            TobiiGameIntegrationApi.Update();
            if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out gazePoint))
            {
                // transform.Translate(new Vector3(gazePoint.X, 0, 0));
                Debug.Log("Got a gaze point at " + gazePoint.TimeStampMicroSeconds + " with coordinates " + gazePoint.X + ", " + gazePoint.Y);
            }

            if (TobiiGameIntegrationApi.TryGetLatestHeadPose(out HeadPose headPose))
            {
                //transform.Translate(new Vector3(gazePoint.X, 0, 0));
                Debug.Log("Got a head pose at " + gazePoint.TimeStampMicroSeconds + " with pose " + headPose.Rotation.YawDegrees + ", " + headPose.Rotation.PitchDegrees + ", " + headPose.Rotation.RollDegrees);
            }

        }
    }
}
