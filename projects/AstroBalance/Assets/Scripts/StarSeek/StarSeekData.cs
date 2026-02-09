/// <summary>
/// Save data for a single star seek session
/// </summary>
[System.Serializable]
public class StarSeekData : GameData
{
    public int timeLimitSeconds;
    public int nStarsCollected;

    //public override string ToCsvHeader()
    //{
    //    string csvHeader = base.ToCsvHeader();
    //    csvHeader += "timeLimitSeconds,nStarsCollected";
    //    return csvHeader;
    //}
}
