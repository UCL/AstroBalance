/// <summary>
/// Save data with overall summary of played sessions
/// </summary>
[System.Serializable]
public class SummaryData : GameData
{
    public int sessionNumber;
    public int totalSessionDurationMinutes;
    public bool rocketLaunchPlayed;
    public bool starCollectorPlayed;
    public bool starSeekPlayed;
    public bool starMapPlayed;
    public bool spaceWalkPlayed;
    public bool zeroGravityPlayed;
    public int totalGamesPlayed;
}
