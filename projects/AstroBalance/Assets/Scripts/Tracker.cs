using UnityEngine;
using Tobii.GameIntegration.Net;
using System.Collections.Generic;
using System;
public class Tracker : MonoBehaviour
{
    private GazePoint gp;
    private HeadPose hp;
    private TobiiRectangle rect;

    private float coordinate_x_scale;
    private float coordinate_y_scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera cam = FindAnyObjectByType<Camera>();
        coordinate_y_scale = cam.orthographicSize;
        coordinate_x_scale = coordinate_y_scale * cam.aspect;

        gp = new GazePoint();
        rect.Left = 0;
        rect.Right = cam.pixelWidth;
        rect.Top = 0;
        rect.Bottom = cam.pixelHeight;

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
    /// Gets most recent gaze point as Unity Coordinates
    /// </summary>
    /// <returns></returns>
    public Vector2 getGazeCoordinates()
    {
        return new Vector2(gp.X * coordinate_x_scale, gp.Y * coordinate_y_scale);
    }

    /// <summary>
    /// Convert an existing gaze point to Unity coordinates
    /// </summary>
    /// <param name="gazepoint"></param>
    /// <returns></returns>
    public Vector2 ConvertGazePointToCoordinates(GazePoint gazepoint)
    {
        return new Vector2(gazepoint.X * coordinate_x_scale, gazepoint.Y * coordinate_y_scale);
    }

    /// <summary>
    /// Convert point in Unity coordinate space to a normalized (-1,1) in x and y
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public Vector2 NormalizeCoordinates(Vector2 coords)
    {
        return new Vector2(coords.x / coordinate_x_scale, coords.y / coordinate_y_scale);
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
    /// Gets the most recently acquired head position information
    /// </summary>
    /// <returns>Head position is {X, Y, Z}</returns>
    public Position getHeadPosittion()
    {
        return hp.Position;
    }
}
