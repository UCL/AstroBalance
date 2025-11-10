
using UnityEngine;
using UnityEngine.InputSystem;
using Tobii.GameIntegration.Net;

public class RocketScript : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Hello World");
#if UNITY_STANDALONE_WIN

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
        Debug.Log(TobiiGameIntegrationApi.GetTrackerInfo("tobii-prp://IS5FF-100214127894"));
        Debug.Log(" Friendly name = " + TobiiGameIntegrationApi.GetTrackerInfo("tobii-prp://IS5FF-100214127894").FriendlyName);
        Debug.Log(" Is it attached ? " + TobiiGameIntegrationApi.GetTrackerInfo("tobii-prp://IS5FF-100214127894").IsAttached);
        Debug.Log(" Track Tracker = " + TobiiGameIntegrationApi.TrackTracker(url));
#endif
    }
    
    void OnDisable()
    {
        TobiiGameIntegrationApi.StopTracking();
    }

    void Update()
    {
        var gamepad = Gamepad.current; //swap this with Tobii when we have it.
        var mouse = Mouse.current;
        TobiiGameIntegrationApi.Update();
        if (gamepad == null)
        {
            if (mouse == null)
            {
                Debug.Log("Update no input");
            }
            else
            {
                if (mouse.leftButton.wasPressedThisFrame)
                {
                if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out GazePoint gazePoint))
                {
                    Debug.Log("Got a gaze point at " + gazePoint.TimeStampMicroSeconds  + " with coordinates " + gazePoint.X + ", " + gazePoint.Y);
                }
                else
                {
                    Debug.Log("Failed to get gaze point");
                }

                Debug.Log(TobiiGameIntegrationApi.GetGazePoints());
                Debug.Log("Mouse pressed");
                }
                else if (mouse.leftButton.wasReleasedThisFrame)
                {
                    Debug.Log("Mouse released");
                }
            }

        }
    }
}
