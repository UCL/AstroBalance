
/// <summary>
/// Base class for a single game's save data.
/// All values that need to be recorded across ALL mini-games should go here.
/// </summary>
[System.Serializable]
public abstract class GameData
{
    public string date = "testdate";
    public string time = "testtime";
    public int secondsPlayed = 5;
    public bool gameCompleted = true;
}
