using UnityEngine;

public class rocket_control : MonoBehaviour
{
    Tracker tracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
    }

    // Update is called once per frame
    void Update()
    {
        var gp = tracker.getGazePoint();
        // transform.Translate(new Vector3(gp.X, gp.Y, 0f));
        // Debug.Log("Rocket control update" + TrackerInterface.getGazePoint()[0]);
    }
}
