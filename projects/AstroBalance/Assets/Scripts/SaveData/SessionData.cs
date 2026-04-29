/// <summary>
/// Save data with overall summary of this session
/// </summary>
[System.Serializable]
public class SessionData : Data
{
    public int sessionNumber;
    public int totalSessionDurationMinutes;
    public bool rocketLaunchPlayed = false;
    public bool starCollectorPlayed = false;
    public bool starSeekPlayed = false;
    public bool starMapPlayed = false;
    public bool spaceWalkPlayed = false;
    public bool zeroGravityPlayed = false;
    public int totalGamesPlayed = 0;
}
