using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Class to save / load data from a particular mini-game.
/// </summary>
/// <typeparam name="T">The type of game data (specific to each mini-game)</typeparam>
[System.Serializable]
public class SaveGameData<T> : SaveData<T>
    where T : GameData, new()
{
    public SaveGameData(string filename)
        : base(filename) { }

    /// <summary>
    /// Get a list of data from the last n complete game sessions (or as many as have been completed so far).
    ///
    /// Game data is stored in chronological order, from earliest to latest (most recent game in final position).
    /// Note: for most mini-games, one data item will be returned per game session - but some (like StarMap) return
    /// multiple items per game session.
    /// </summary>
    /// <param name="nSessions">Maximum number of game sessions to retrieve</param>
    public IEnumerable<T> GetLastNCompleteSessions(int nSessions)
    {
        List<T> lastNSessionData = new List<T>();
        int nSessionsDataRetrieved = 0;
        int currentSessionNumber = -1;

        if (!saveFileExists)
        {
            return lastNSessionData;
        }

        IEnumerable<string> csvLines = File.ReadLines(dataPath);
        string header = csvLines.First();
        int maxLineNo = csvLines.Count() - 1;

        // Start from end of file, and find n complete game sessions
        for (int i = maxLineNo; i > 0; i--)
        {
            string line = csvLines.ElementAt(i);
            T gameData = CsvToData(header, line);

            if (gameData.gameCompleted)
            {
                if (gameData.sessionNumber != currentSessionNumber)
                {
                    nSessionsDataRetrieved++;
                }
                if (nSessionsDataRetrieved > nSessions)
                {
                    break;
                }

                lastNSessionData.Add(gameData);
                currentSessionNumber = gameData.sessionNumber;
            }
        }

        lastNSessionData.Reverse();
        return lastNSessionData;
    }
}
