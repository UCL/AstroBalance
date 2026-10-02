/// <summary>
/// Save data for a single rocket launch session
/// </summary>
[System.Serializable]
public class RocketLaunchData : GameData
{
    public int difficultyLevel;
    public string headMovementPlane;
    public int gameDurationSeconds;
    public float launchTimeSeconds;
    public float minimumHeadSpeed;
    public float gazeTolerance;
    public float headSpeedDegPerSecMean;
    public float headSpeedDegPerSecPeak;
    public float headSpeedDegPerSecMedian;
    public float headSpeedDegPerSecSD;
    public float percentTimeAbove40DegPerSec;
    public float percentTimeGazeOnTarget;
    public float timeInAdaptationWindow1;
    public float timeInAdaptationWindow2;
    public float timeInAdaptationWindow3;
    public float timeInAdaptationWindow4;
}
