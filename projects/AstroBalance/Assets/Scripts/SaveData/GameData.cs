using System;

/// <summary>
/// Base class for a single game's save data.
/// All values that need to be recorded across ALL mini-games should go here.
/// </summary>
[System.Serializable]
public abstract class GameData : Data
{
    public int gameNumber = 1;
    public bool gameCompleted = false;
}
