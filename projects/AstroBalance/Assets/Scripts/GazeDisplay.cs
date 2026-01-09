using TMPro;
using UnityEngine;

public class GazeDisplay : MonoBehaviour
{
    private Tracker tracker;
    TextMeshProUGUI gazeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        gazeText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var gp = tracker.getGazePoint();
        var gpWorld = tracker.getGazeWorldCoordinates();


        gazeText.text = "Gaze Point: (" + gp.X + ", " + gp.Y + ") \n";
        gazeText.text += "Unity coords: (" + gpWorld.x + ", " + gpWorld.y + ") \n";
    }
}
