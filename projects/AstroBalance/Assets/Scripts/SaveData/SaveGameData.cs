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
    /// Get a list of data from the last n complete played games (or as many as have been completed so far).
    ///
    /// Game data is stored in chronological order, from earliest to latest (most recent game in final position).
    /// Note: for most mini-games, one data item will be returned per game - but some (like StarMap) return
    /// multiple items per game.
    /// </summary>
    /// <param name="nGames">Maximum number of games to retrieve</param>
    public IEnumerable<T> GetLastNComplete(int nGames)
    {
        List<T> lastNComplete = new List<T>();
        int nGamesDataRetrieved = 0;
        int currentGameNumber = -1;

        if (!saveFileExists)
        {
            return lastNComplete;
        }

        IEnumerable<string> csvLines = File.ReadLines(dataPath);
        string header = csvLines.First();
        int maxLineNo = csvLines.Count() - 1;

        // Start from end of file, and find n complete played games
        for (int i = maxLineNo; i > 0; i--)
        {
            string line = csvLines.ElementAt(i);
            T gameData = CsvToData(header, line);

            if (gameData.gameCompleted)
            {
                if (gameData.gameNumber != currentGameNumber)
                {
                    nGamesDataRetrieved++;
                }
                if (nGamesDataRetrieved > nGames)
                {
                    break;
                }

                lastNComplete.Add(gameData);
                currentGameNumber = gameData.gameNumber;
            }
        }

        lastNComplete.Reverse();
        return lastNComplete;
    }

    /// <summary>
    /// Get next available game number.
    /// </summary>
    public int GetNextGameNumber()
    {
        T lastGame = GetLast();
        return lastGame is not null ? lastGame.gameNumber + 1 : 1;
    }
}
