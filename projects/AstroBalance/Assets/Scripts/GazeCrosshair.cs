using UnityEngine;
using Tobii.GameIntegration.Net;

public class GazeCrosshair : MonoBehaviour
{
    public Camera activeCamera;
    private Tracker tracker;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>(); 
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 gazePointPixels = tracker.getGazePointDisplayPixels();
        //Vector3 worldPoint = activeCamera.ScreenToWorldPoint(gazePointPixels);
        Vector2 gazePointViewport = tracker.getGazePointViewport();
        Vector3 worldPoint = activeCamera.ViewportToWorldPoint(gazePointViewport);
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
    }
}
