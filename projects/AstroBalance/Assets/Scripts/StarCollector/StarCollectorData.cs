/// <summary>
/// Save data for a single star collector session
/// </summary>
[System.Serializable]
public class StarCollectorData : GameData
{
    public int timeLimitSeconds;
    public int nStarsCollected;
    public float percentStarsCollected;
}
