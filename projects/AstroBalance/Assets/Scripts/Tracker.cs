using System.Collections.Generic;
using Tobii.GameIntegration.Net;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private GazePoint gp;
    private HeadPose hp;
    private TobiiRectangle rect;
    private bool playerDetected;
    private int screenWidthMm;
    private int screenHeightMm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        gp = new GazePoint();
        rect.Left = 0;
        rect.Right = Camera.main.pixelWidth;
        rect.Top = 0;
        rect.Bottom = Camera.main.pixelHeight;

        Debug.Log($"Initialised = {TobiiGameIntegrationApi.IsApiInitialized()}");
        TobiiGameIntegrationApi.Update();
        TobiiGameIntegrationApi.UpdateTrackerInfos();

        Debug.Log($"Track? = {TobiiGameIntegrationApi.TrackRectangle(rect)}");
        Debug.Log($"Is connected? = {TobiiGameIntegrationApi.IsTrackerConnected()}");
        Debug.Log($"Is enabled? = {TobiiGameIntegrationApi.IsTrackerEnabled()}");

        // UpdateTrackerInfos() starts a scan that takes a while to finish. Until it is complete,
        // GetTrackerInfos will return null
        List<TrackerInfo> trackerInfos = null;
        while (trackerInfos == null)
        {
            trackerInfos = TobiiGameIntegrationApi.GetTrackerInfos();
        }

        WidthHeight displaySizeMm = trackerInfos[0].DisplaySizeMm;
        screenWidthMm = displaySizeMm.Width;
        screenHeightMm = displaySizeMm.Height;
        Debug.Log($"Detected screen size (mm) = {screenWidthMm}, {screenHeightMm}");

        // call refresh once during awake, to ensure tracking data is available immediately
        RefreshTrackingData();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void OnDestroy()
    {
        //TobiiGameIntegrationApi.Shutdown();
    }

    // Update is called once per frame
    void Update()
    {
        RefreshTrackingData();
    }

    private void RefreshTrackingData()
    {
        TobiiGameIntegrationApi.Update();
        TobiiGameIntegrationApi.TryGetLatestGazePoint(out gp);
        TobiiGameIntegrationApi.TryGetLatestHeadPose(out hp);
        playerDetected = TobiiGameIntegrationApi.IsPresent();
    }

    /// <summary>
    /// Return true if the eye tracker detects the player is present.
    /// </summary>
    /// <returns>true/false</returns>
    public bool isPlayerDetected()
    {
        return playerDetected;
    }

    /// <summary>
    /// Gets most recent gaze point information. (0, 0) is the centre
    /// of the display, with (-1, -1) the bottom left corner and (1, 1)
    /// the top right.
    /// </summary>
    /// <returns>Gaze point is {TimeStampMicroSeconds, X, Y}</returns>
    public GazePoint getGazePoint()
    {
        return gp;
    }

    /// <summary>
    /// Gets most recent gaze point information, in unity viewport
    /// coordinates. (0,0) is the bottom left and (1, 1) is the top right.
    /// Unity must be displayed full screen for coordinates to match.
    /// </summary>
    /// <returns>Gaze point as a Vector2 {X, Y}</returns>
    public Vector2 getGazeViewportCoordinates()
    {
        return ConvertGazePointToViewportCoordinates(gp);
    }

    /// <summary>
    /// Gets most recent gaze point information, in unity world
    /// coordinates.
    /// Unity must be displayed full screen for coordinates to match.
    /// </summary>
    /// <returns>Gaze point as a Vector2 {X, Y}</returns>
    public Vector2 getGazeWorldCoordinates()
    {
        return ConvertGazePointToWorldCoordinates(gp);
    }

    /// <summary>
    /// Convert an existing gaze point to Unity viewport coordinates.
    /// </summary>
    /// <param name="gazepoint">gaze point to convert</param>
    /// <returns>Converted gaze point as a Vector2 {X, Y}</returns>
    public Vector2 ConvertGazePointToViewportCoordinates(GazePoint gazepoint)
    {
        Vector2 gazePointViewport = new Vector2(gazepoint.X, gazepoint.Y);
        gazePointViewport.x = (gazePointViewport.x + 1) / 2;
        gazePointViewport.y = (gazePointViewport.y + 1) / 2;
        gazePointViewport = clipToRange(gazePointViewport, 0, 1);

        return gazePointViewport;
    }

    /// <summary>
    /// Convert an existing gaze point to Unity world coordinates
    /// </summary>
    /// <param name="gazepoint">gaze point to convert</param>
    /// <returns>Converted gaze point as a Vector2 {X, Y}</returns>
    public Vector2 ConvertGazePointToWorldCoordinates(GazePoint gazepoint)
    {
        Vector2 gazePointViewport = ConvertGazePointToViewportCoordinates(gazepoint);
        return Camera.main.ViewportToWorldPoint(gazePointViewport);
    }

    /// <summary>
    /// Convert point in Unity world coordinate space to a normalized (-1,1) in x and y
    /// </summary>
    /// <param name="coords">Unity world coordinate as Vector2 {X, Y}</param>
    /// <returns>Normalised gaze point as Vector2 {X, Y}</returns>
    public Vector2 NormalizeCoordinates(Vector2 coords)
    {
        Vector2 gazePoint = Camera.main.WorldToViewportPoint(coords);
        gazePoint.x = (gazePoint.x * 2) - 1;
        gazePoint.y = (gazePoint.y * 2) - 1;
        gazePoint = clipToRange(gazePoint, -1, 1);

        return gazePoint;
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
    /// Gets the most recently acquired head position information.
    /// Position is measured in mm, with (0, 0, 0) at the centre of the screen.
    /// </summary>
    /// <returns>Head position is {X, Y, Z}</returns>
    public Position getHeadPosition()
    {
        return hp.Position;
    }

    /// <summary>
    /// Gets most recent head point information, in unity viewport
    /// coordinates. The 'head point' is the position on the screen based on head
    /// pose alone i.e. assuming the user is looking straight ahead.
    ///
    /// (0,0) is the bottom left and (1, 1) is the top right.
    /// Unity must be displayed full screen for coordinates to match.
    /// </summary>
    /// <returns>Head point as a Vector2 {X, Y}</returns>
    public Vector2 getHeadViewportCoordinates()
    {
        Position headPosition = hp.Position;
        Rotation headRotation = hp.Rotation;

        // The calculated xPosition / yPosition below assumes the player has their head
        // positioned near the centre of the screen in x/y. If required, we could consider
        // compensating for the measured headPosition.x/y for higher accuracy.

        // Base x position on the yaw angle
        float xPositionMillimetre =
            Mathf.Tan(hp.Rotation.YawDegrees * Mathf.Deg2Rad) * headPosition.Z;
        // Unity viewport centre is at (0.5, 0.5)
        float xPositionViewport = 0.5f + (xPositionMillimetre / screenWidthMm);

        // Base y position on the pitch angle
        float yPositionMillimetre =
            Mathf.Tan(hp.Rotation.PitchDegrees * Mathf.Deg2Rad) * headPosition.Z;
        float yPositionViewport = 0.5f + (yPositionMillimetre / screenHeightMm);

        Vector2 headPointViewport = new Vector2(xPositionViewport, yPositionViewport);
        headPointViewport = clipToRange(headPointViewport, 0, 1);

        return headPointViewport;
    }

    /// <summary>
    /// Gets most recent head point information, in unity world
    /// coordinates. The 'head point' is the position on the screen based on head
    /// pose alone i.e. assuming the user is looking straight ahead.
    /// Unity must be displayed full screen for coordinates to match.
    /// </summary>
    /// <returns>Head point as a Vector2 {X, Y}</returns>
    public Vector2 getHeadWorldCoordinates()
    {
        Vector2 headPointViewport = getHeadViewportCoordinates();
        return Camera.main.ViewportToWorldPoint(headPointViewport);
    }

    /// <summary>
    /// Clip coordinates outside the given range to min-max
    /// </summary>
    private Vector2 clipToRange(Vector2 vector, float min, float max)
    {
        if (vector.x < min)
        {
            vector.x = min;
        }
        if (vector.y < min)
        {
            vector.y = min;
        }
        if (vector.x > max)
        {
            vector.x = max;
        }
        if (vector.y > max)
        {
            vector.y = max;
        }

        return vector;
    }
}
