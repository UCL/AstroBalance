using System.Linq;

/// <summary>
/// Save data with overall summary of this session
/// </summary>
[System.Serializable]
public class SessionData : Data
{
    public int sessionNumber;
    public string totalSessionDuration;
    public bool game1RocketLaunchPlayed = false;
    public bool game2StarCollectorPlayed = false;
    public bool game3StarSeekPlayed = false;
    public bool game4StarMapPlayed = false;
    public bool game5SpaceWalkPlayed = false;
    public bool game6ZeroGravityPlayed = false;
    public int totalGamesPlayed = 0;

    public void UpdateTotalGamesPlayed()
    {
        bool[] gamesPlayed =
        {
            game1RocketLaunchPlayed,
            game2StarCollectorPlayed,
            game3StarSeekPlayed,
            game4StarMapPlayed,
            game5SpaceWalkPlayed,
            game6ZeroGravityPlayed,
        };

        totalGamesPlayed = gamesPlayed.Count(c => c);
    }
}
