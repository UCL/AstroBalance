/// <summary>
/// Save data for a single zero gravity pose
/// </summary>
[System.Serializable]
public class ZeroGravityData : GameData
{
    public int poseNumber;
    public string poseType;
    public int poseTimeLimitSeconds;
    public int poseDurationSeconds;
    public int? balanceStabilityScore; // null if the player exited before pose scoring started
    public int? falls; // null if the player exited before pose scoring started
}
