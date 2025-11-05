
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
        Debug.Log(TobiiGameIntegrationApi.GetTrackerInfo("tobii-prp://IS5FF-100214127894"));
        Debug.Log(TobiiGameIntegrationApi.GetTrackerInfo("tobii-prp://IS5FF-100214127894").FriendlyName);
        Debug.Log(TobiiGameIntegrationApi.GetTrackerInfo("tobii-prp://IS5FF-100214127894").IsAttached);
        #endif
    }

    void Update()
    {
        var gamepad = Gamepad.current; //swap this with Tobii when we have it.
        var mouse = Mouse.current;
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
