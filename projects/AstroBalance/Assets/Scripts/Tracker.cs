using UnityEngine;
using Tobii.GameIntegration.Net;
using System.Collections.Generic;
using System;
public class Tracker : MonoBehaviour
{
    private GazePoint gp;
    private HeadPose hp;
    private TobiiRectangle rect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gp = new GazePoint();
        rect.Left = 0;
        rect.Right = 1920;
        rect.Top = 0;
        rect.Bottom = 1080;

        Debug.Log($"Initialised = {TobiiGameIntegrationApi.IsApiInitialized()}");
        TobiiGameIntegrationApi.Update();
        TobiiGameIntegrationApi.UpdateTrackerInfos();
        List<TrackerInfo> tis = TobiiGameIntegrationApi.GetTrackerInfos();
        //Debug.Log($"Any tracker info? {tis[0].DisplayRectInOSCoordinates.Top}, {tis[0].DisplayRectInOSCoordinates.Bottom}, {tis[0].DisplayRectInOSCoordinates.Left}, {tis[0].DisplayRectInOSCoordinates.Right}");
        Debug.Log($"Track? = {TobiiGameIntegrationApi.TrackRectangle(rect)}");

        Debug.Log($"Is conncected? = {TobiiGameIntegrationApi.IsTrackerConnected()}");
        Debug.Log($"Is enabled? = {TobiiGameIntegrationApi.IsTrackerEnabled()}");
    }

    private void OnDestroy()
    {
        //TobiiGameIntegrationApi.Shutdown();
    }

    // Update is called once per frame
    void Update()
    {
        TobiiGameIntegrationApi.Update();

        if (TobiiGameIntegrationApi.TryGetLatestGazePoint(out gp))
        {
            //Debug.Log($"Gaze point = {gp.X}, {gp.Y}");
        }

        TobiiGameIntegrationApi.TryGetLatestHeadPose(out hp);
    }

    /// <summary>
    /// Gets most recent gaze point information
    /// </summary>
    /// <returns>Gaze point is {TimeStampMicroSeconds, X, Y}</returns>
    public GazePoint getGazePoint()
    {
        return gp;
    }

    /// <summary>
    /// Gets all most recent head pose information
    /// </summary>
    /// <returns>Head pose is {Rotation, Position, TimeStampMicroSeconds}</returns>
    public HeadPose getHeadPose()
    {
        return hp;
    }

    /// <summary>
    /// Gets the most recently acquired head rotation information
    /// </summary>
    /// <returns>Rotation is {YawDegrees, PitchDegrees, RollDegrees}</returns>
    public Rotation getHeadRotation()
    {
        return hp.Rotation;
    }

    /// <summary>
    /// Gets the msot recently acquired head position information
    /// </summary>
    /// <returns>Head posotion is {X, Y, Z}</returns>
    public Position getHeadPosittion()
    {
        return hp.Position;
    }
}
