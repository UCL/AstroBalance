using Tobii.GameIntegration.Net;
using UnityEngine;

public class SwayLine : MonoBehaviour
{
    [SerializeField, Tooltip("Head x movement scaling (moving 1mm, moves this many unity units)")]
    private float headXScaling = 0.01f;

    private Tracker tracker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();   
    }

    // Update is called once per frame
    void Update()
    {
        HeadPose currentHeadPose = tracker.getHeadPose();
        float xPosMm = currentHeadPose.Position.X;
        float rollDegrees = currentHeadPose.Rotation.RollDegrees;

        // We only move the sway line on the x axis - left/right (we don't care about
        // changes in head height, or distance from screen)
        transform.position = new Vector3(xPosMm * headXScaling, transform.position.y, 0);

        // We rotate only with head roll
        transform.eulerAngles = new Vector3(0, 0, -rollDegrees);
    }
}
