/// <summary>
/// Save data for a single rocket launch session
/// </summary>
[System.Serializable]
public class RocketLaunchData : GameData
{
    public string headMovementPlane;
    public int gameDurationSeconds;
    public float launchTimeSeconds;
    public float minimumHeadSpeed;
    public float gazeTolerance;
    public float headSpeedDegPerSecMean;
    public float headSpeedDegPerSecPeak;
    public float headSpeedDegPerSecMedian;
    public float headSpeedDegPerSecSD;
}
