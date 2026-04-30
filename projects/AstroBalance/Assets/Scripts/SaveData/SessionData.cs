/// <summary>
/// Save data with overall summary of this session
/// </summary>
[System.Serializable]
public class SessionData : Data
{
    public string totalSessionDuration;
    public int nCompleteRocketLaunchGames = 0;
    public int nCompleteStarCollectorGames = 0;
    public int nCompleteStarSeekGames = 0;
    public int nCompleteStarMapGames = 0;
    public int nCompleteSpaceWalkGames = 0;
    public int nCompleteZeroGravityGames = 0;
    public int totalCompleteGames = 0;

    public void UpdateTotalCompleteGames()
    {
        totalCompleteGames =
            nCompleteRocketLaunchGames
            + nCompleteStarCollectorGames
            + nCompleteStarSeekGames
            + nCompleteStarMapGames
            + nCompleteSpaceWalkGames
            + nCompleteZeroGravityGames;
    }
}
