/// <summary>
/// Save data for a single star collector session
/// </summary>
[System.Serializable]
public class StarCollectorData : GameData
{
    public int timeLimitSeconds;
    public int gameDurationSeconds;
    public int nStarsCollected;
    public float percentStarsCollected;
    public int adaptiveLevel;
    public int finalStarFallSpeed;
    public float headVelocityDegPerSecMean;
    public float headVelocityDegPerSecPeak;
    public float headVelocityDegPerSecSD;
}
