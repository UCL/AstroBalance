/// <summary>
/// Save data for a single space walking session
/// </summary>
[System.Serializable]
public class SpaceWalkingData : GameData
{
    public int timeLimitSeconds;
    public int nCompleteSteps; // one complete step = a step out + back to the centre
    public bool headTurnsActive;
}
