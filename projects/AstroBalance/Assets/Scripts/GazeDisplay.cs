using TMPro;
using UnityEngine;

public class GazeDisplay : MonoBehaviour
{
    [SerializeField] private Tracker tracker;
    TextMeshProUGUI tmp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var gp = tracker.getGazePoint();
        tmp.text = "Gaze Point: (" + gp.X.ToString() + ", " + gp.Y.ToString() + ")";
    }
}
