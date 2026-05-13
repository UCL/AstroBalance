/// <summary>
/// Save data for a single rocket launch session
/// </summary>
[System.Serializable]
public class RocketLaunchData : GameData
{
    public string headMovementPlane;
    public int gameDurationSeconds;
    public float launchTimeSeconds;
}
