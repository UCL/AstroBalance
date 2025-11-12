
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
        
        TrackerInfo trackerInfo = TobiiGameIntegrationApi.GetTrackerInfo();
         if (trackerInfo.Url != "")
         {
            Debug.Log("Found tracker: " + trackerInfo.FriendlyName + " at " + trackerInfo.Url);
            Debug.Log(" Is it attached ? " + trackerInfo.IsAttached);
         }
         else
         {
             Debug.Log("No tracker found");
         }
    
        string myurl = trackerInfo.Url; 
        
        // the trackerTracker call seems to be what starts the tracking.
        Debug.Log(" Track Tracker = " + TobiiGameIntegrationApi.TrackTracker(myurl));
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
