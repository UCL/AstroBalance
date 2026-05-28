using System.Collections.Generic;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class GazeRefresh : MonoBehaviour
{
    private GazeBuffer gazeBuffer;
    private Tracker tracker;
    private float samplingInterval = 0.5f;
    private List<bool> steady = new();

    private float timeToNextSample;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tracker = FindFirstObjectByType<Tracker>();
        gazeBuffer = new GazeBuffer(200, 2);

        timeToNextSample = samplingInterval;
    }

    // Update is called once per frame
    void Update()
    {
        GazePoint gazePoint = tracker.getGazePoint();
        GazeItem gazeItem = new();
        gazeItem.gazePoint = gazePoint;
        gazeBuffer.addIfNew(gazeItem);

        if (timeToNextSample > 0)
        {
            timeToNextSample -= Time.deltaTime;
        }
        else
        {
            steady.Add(gazeBuffer.gazeSteady(samplingInterval, 100, 0, 0));
            timeToNextSample = samplingInterval;
        }
    }
}
