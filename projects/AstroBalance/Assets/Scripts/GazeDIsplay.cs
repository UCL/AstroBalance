using TMPro;
using UnityEngine;

public class GazeDIsplay : MonoBehaviour
{
    TextMeshProUGUI gazeText;
    [SerializeField] Tracker tracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gazeText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var gp = tracker.getGazePoint();
        gazeText.text = "Gaze Point: " + gp.X.ToString() + ", " + gp.Y.ToString();
    }
}
