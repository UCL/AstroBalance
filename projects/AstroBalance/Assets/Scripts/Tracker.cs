using System.Collections.Generic;
using Tobii.GameIntegration.Net;
using UnityEngine;
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
        rect.Right = Camera.main.pixelWidth;
        rect.Top = 0;
        rect.Bottom = Camera.main.pixelHeight;

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
    /// This uses the most recent head (0,0) is the bottom left and (1, 1) is the top right.
    /// Unity must be displayed full screen for coordinates to match.
    /// </summary>
    /// <returns>Gaze point as a Vector2 {X, Y}</returns>
    public Vector2 getHeadPointViewport()
    {
        Position headPosition = hp.Position;
        Rotation headRotation = hp.Rotation;

        //float dpi = Screen.dpi;
        //float screenWidthInches = (rect.Right - rect.Left) / dpi;
        //float screenHeightInches = (rect.Bottom - rect.Top) / dpi;
        //float screenWidthMillimetre = screenWidthInches * 25.4f;
        //float screenHeightMillimetre = screenHeightInches * 25.4f;

        // Tried getting screen width from pixel width (1920) / Screen.dpi
        // but Screen.dpi doesn't seem to be physically accurate and width
        // was far off correct - hard code for now
        float screenWidthMillimetre = 525;
        float screenHeightMillimetre = 295;

        //Debug.Log(screenWidthMillimetre);

        // Base x position on the yaw angle - assuming the player is positioned
        // with their head near the centre of the screen (on the x axis, i.e left-right).
        // If required, we could consider compensating for the measured headPosition.X also
        // for higher accuracy.
        float xPositionMillimetre = Mathf.Tan(hp.Rotation.YawDegrees * Mathf.Deg2Rad) * headPosition.Z;
        // Unity viewport centre is at (0.5, 0.5)
        float xPositionViewport = 0.5f + (xPositionMillimetre / screenWidthMillimetre);

        // Base y position on the pitch angle - taking into account the current head height (y).
        // We must compensate for head height, as e.g. looking down at a laptop screen vs up at
        // an external monitor will give quite different points.

        // y position always feels a bit high for the corresponding head position; offset by a fixed number of degrees 
        // to compensate
        //float pitchOffset = -10;
        float pitchOffset = 0;
        float yPositionMillimetre = Mathf.Tan((hp.Rotation.PitchDegrees + pitchOffset) * Mathf.Deg2Rad) * headPosition.Z;
        //Debug.Log("y pos mm" + yPositionMillimetre);
        //Debug.Log("head position y" + headPosition.Y);
        //yPositionMillimetre += headPosition.Y;
        float yPositionViewport = 0.5f + (yPositionMillimetre / screenHeightMillimetre);

        Vector2 headPointViewport = new Vector2(xPositionViewport, yPositionViewport);
        headPointViewport = clipToRange(headPointViewport, 0, 1);

        return headPointViewport;
    }

    /// <summary>
    /// Clip coordinates outside the given range to min-max
    /// </summary>
    private Vector2 clipToRange(Vector2 vector, float min, float max)
    {
        if (vector.x < min) { vector.x = min; }
        if (vector.y < min) { vector.y = min; }
        if (vector.x > max) { vector.x = max; }
        if (vector.y > max) { vector.y = max; }

        return vector;
    }
}
