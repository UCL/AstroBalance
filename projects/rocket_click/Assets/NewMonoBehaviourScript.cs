
using UnityEngine;
using UnityEngine.InputSystem;
using Tobii.GameIntegration.Net;

public class RocketScript : MonoBehaviour
{
    bool usingTobii = false;
    void Start()
    {
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

        // I've added the device URL (found using the tracker info example) to the following calls. They now seem 
        // work. TODO - find a more robust way of getting URL
        string url = "tobii-prp://IS5FF-100214127894";
        
        Debug.Log(" Friendly name = " + TobiiGameIntegrationApi.GetTrackerInfo(url).FriendlyName);
        Debug.Log(" Is it attached ? " + TobiiGameIntegrationApi.GetTrackerInfo(url).IsAttached);
        // the trackerTracker call seems to be what starts the tracking.
        Debug.Log(" Track Tracker = " + TobiiGameIntegrationApi.TrackTracker(url));
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
            // the following works for either pose or gaze point, curiously not at the same time though.
            TobiiGameIntegrationApi.Update();
            if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out GazePoint gazePoint))
            {
              transform.Translate(new Vector3(gazePoint.X, 0, 0));
              Debug.Log("Got a gaze point at " + gazePoint.TimeStampMicroSeconds + " with coordinates " + gazePoint.X + ", " + gazePoint.Y);
            }
            TobiiGameIntegrationApi.Update();
            if (TobiiGameIntegrationApi.TryGetLatestHeadPose(out HeadPose headPose))
            {
                //transform.Translate(new Vector3(gazePoint.X, 0, 0));
                Debug.Log("Got a head pose at " + gazePoint.TimeStampMicroSeconds + " with pose " + headPose.Rotation.YawDegrees + ", " + headPose.Rotation.PitchDegrees + ", " + headPose.Rotation.RollDegrees);
            }
            TobiiGameIntegrationApi.Update();
        }
    }
}
