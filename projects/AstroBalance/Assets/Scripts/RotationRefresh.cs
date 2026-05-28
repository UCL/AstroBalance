using System.Collections.Generic;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class RotationRefresh : MonoBehaviour
{
    private HeadPoseBuffer headPoseBuffer;
    private Tracker tracker;
    private float samplingInterval = 0.5f;
    private List<float> speeds = new();

    private float timeToNextSample;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        headPoseBuffer = new HeadPoseBuffer(100, 2);

        timeToNextSample = samplingInterval;
    }

    // Update is called once per frame
    void Update()
    {
        HeadPose headPose = tracker.getHeadPose();
        headPoseBuffer.addIfNew(new HeadPoseItem(headPose));

        if (timeToNextSample > 0)
        {
            timeToNextSample -= Time.deltaTime;
        }
        else
        {
            speeds.Add(headPoseBuffer.getSpeed(samplingInterval, HeadPoseAxis.Yaw));
            timeToNextSample = samplingInterval;
        }
    }
}
