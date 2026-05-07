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
    public float meanSwayVelocityCmPerSec;
    public int balanceStabilityScore;
    public bool poseCompletedSuccessfully;
    public int falls;
}
