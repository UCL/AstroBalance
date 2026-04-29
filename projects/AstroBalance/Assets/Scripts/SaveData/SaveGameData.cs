using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Class to save / load data from multiple game sessions.
/// </summary>
/// <typeparam name="T">The type of game data (specific to each mini-game)</typeparam>
[System.Serializable]
public class SaveGameData<T> : SaveData<T>
    where T : GameData, new()
{
    public SaveGameData(string filename)
        : base(filename) { }

    /// <summary>
    /// Get data from the last complete played game session.
    /// </summary>
    public T GetLastComplete()
    {
        if (!saveFileExists)
        {
            return null;
        }

        IEnumerable<T> lastGameData = GetLastNComplete(1);
        if (lastGameData.Count() != 1)
        {
            return null;
        }
        else
        {
            return lastGameData.ElementAt(0);
        }
    }

    /// <summary>
    /// Get a list of data from the last n complete played games (or as many as have been completed). Game data is stored in chronological order, from earliest to latest (most recent game in final position).
    /// </summary>
    /// <param name="nGames">Maximum number of games to retrieve</param>
    public IEnumerable<T> GetLastNComplete(int nGames)
    {
        List<T> lastNComplete = new List<T>();
        if (!saveFileExists)
        {
            return lastNComplete;
        }

        IEnumerable<string> csvLines = File.ReadLines(dataPath);
        string header = csvLines.First();
        int lineNo = csvLines.Count() - 1;

        // Start from end of file, and find n complete games
        while (lineNo > 0 && lastNComplete.Count() < nGames)
        {
            string line = csvLines.ElementAt(lineNo);

            T gameData = CsvToData(header, line);
            if (gameData.gameCompleted)
            {
                lastNComplete.Add(gameData);
            }
            lineNo--;
        }

        lastNComplete.Reverse();

        return lastNComplete;
    }
}
