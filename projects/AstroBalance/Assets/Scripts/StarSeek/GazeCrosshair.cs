using UnityEngine;

public class GazeCrosshair : MonoBehaviour
{
    private Tracker tracker;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>(); 
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 worldPoint;
        if (Application.isEditor)
        {
            // Game window needs to be full screen for eye tracking (gaze point) to work correctly.
            // For easier debugging, when shown in small editor view, have the Crosshair follow the
            // mouse instead.
            Vector3 mousePosition = Input.mousePosition;
            worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        else
        {
            Vector2 gazePointViewport = tracker.getGazePointViewport();
            worldPoint = Camera.main.ViewportToWorldPoint(gazePointViewport);
        }
        
        transform.position = new Vector3(worldPoint.x, worldPoint.y, transform.position.z);
    }
}
