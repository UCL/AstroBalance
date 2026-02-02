/// <summary>
/// Save data for a single rocket launch session
/// </summary>
[System.Serializable]
public class RocketLaunchData : GameData
{
    public bool pitch; // true if head pitch was used, false if yaw was used
    public int launchTimeSeconds;
}
