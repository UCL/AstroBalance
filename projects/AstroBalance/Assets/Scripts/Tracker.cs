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

        Debug.Log($"Is connected? = {TobiiGameIntegrationApi.IsTrackerConnected()}");
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
    /// Gets most recent gaze point information, in pixel units of the 
    /// entire display. (0,0) is bottom left pixel of the monitor.
    /// </summary>
    /// <returns>Gaze point as a Vector2Int {X, Y}</returns>
    public Vector2Int getGazePointDisplayPixels()
    {
        Debug.Log("gaze point x" + gp.X);
        Debug.Log("gaze point y" + gp.Y);

        int maxX = 1920;
        int maxY = 1080;

        // (int) is equivalent to taking the Math.floor, 
        // as we neeed to round down to the nearest whole pixel
        int xCoord = (int)(((gp.X + 1) / 2) * maxX);
        int yCoord = (int)(((gp.Y + 1) / 2) * maxY);

        if (xCoord < 0) { xCoord = 0; }
        if (yCoord < 0) { yCoord = 0; }

        if (xCoord > maxX) {  xCoord = maxX; }
        if (yCoord > maxY) { yCoord = maxY; }

        Debug.Log("x coordinate" + xCoord);
        Debug.Log("y coordiante" + yCoord);

        return new Vector2Int(xCoord, yCoord);
    }

    public Vector2 getGazePointScreenPixels()
    {
        Vector2Int gpDisplayPixels = getGazePointDisplayPixels();

        // Location of top left corner of unity screen, relative to display top left corner
        Vector2Int topLeftUnity = Screen.mainWindowPosition;

        // We need location of bottom left corner, relative to display bottom left
        Vector2Int bottomLeftUnity = new Vector2Int(topLeftUnity.x, 1080 - (topLeftUnity.y + Screen.height));

        Debug.Log("top left corner" + topLeftUnity);
        Debug.Log("bottom left corner" + bottomLeftUnity);
        Debug.Log("screen height" + Screen.height);
        Debug.Log("screen width" + Screen.width);

        Vector2Int gpScreenPixels = new Vector2Int(
            gpDisplayPixels.x - bottomLeftUnity.x,
            gpDisplayPixels.y - bottomLeftUnity.y
        );

        Debug.Log("screen pixels" + gpScreenPixels);

        int maxX = bottomLeftUnity.x + Screen.width;
        int maxY = 1080 - topLeftUnity.y;

        if (gpScreenPixels.x < 0) gpScreenPixels.x = 0;
        if (gpScreenPixels.y < 0) gpScreenPixels.y = 0;
        if (gpScreenPixels.x > maxX) gpScreenPixels.x = maxX;
        if (gpScreenPixels.y > maxY) gpScreenPixels.y = maxY;

        return gpScreenPixels;
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
