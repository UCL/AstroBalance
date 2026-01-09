using TMPro;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class PoseDisplay : MonoBehaviour
{
    private Tracker tracker;
    TextMeshProUGUI poseText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        poseText = GetComponent<TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        HeadPose headPose = tracker.getHeadPose();
        Position position = headPose.Position;
        Rotation rotation = headPose.Rotation;

        poseText.text = "Head position: (" + position.X + ", " + position.Y + ", " + position.Z + ") \n";
        poseText.text += "Head rotation: (" + rotation.RollDegrees + ", " + rotation.PitchDegrees + ", " + rotation.YawDegrees + ") \n";
    }
}
