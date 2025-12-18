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
        var gpDisplay = tracker.getGazePointDisplayPixels();
        var gpScreen = tracker.getGazePointScreenPixels();

        gazeText.text = "Gaze Point: (" + gp.X + ", " + gp.Y + ") \n";
        gazeText.text += "Display coords: (" + gpDisplay.x + ", " + gpDisplay.y + ") \n";
        gazeText.text += "Screen coords: (" + gpScreen.x + ", " + gpScreen.y + ")";
    }
}
