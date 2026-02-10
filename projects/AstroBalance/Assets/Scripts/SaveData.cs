using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class to save / load data from multiple game sessions.
/// </summary>
/// <typeparam name="T">The type of game data (specific to each mini-game)</typeparam>
[System.Serializable]
public class SaveData<T>
    where T : GameData, new()
{
    public bool saveFileExists = false;
    public List<T> savedGames = new List<T>();
    private string dataPath;

    /// <summary>
    /// Create a new Save Data collection. If available, this will
    /// be build on previous save data from filename.csv
    /// </summary>
    /// <param name="filename">The filename of the save file</param>
    public SaveData(string filename)
    {
        // currently defaults to "C:\Users\username\AppData\LocalLow\DefaultCompany\AstroBalance" on Windows
        dataPath = Path.Combine(Application.persistentDataPath, filename + ".csv");
        CheckSaveFileExists();
    }

    /// <summary>
    /// Add data from a new game session to the save file.
    /// </summary>
    /// <param name="gameData">Game data from this session</param>
    public void SaveGameData(T gameData)
    {
        using (StreamWriter sw = new StreamWriter(dataPath, true))
        {
            if (!saveFileExists)
            {
                sw.WriteLine(gameData.ToCsvHeader());
                saveFileExists = true;
            }

            sw.WriteLine(gameData.ToCsvRow());
        }
    }

    /// <summary>
    /// Get data from the last complete played game session.
    /// </summary>
    public T GetLastCompleteGameData()
    {
        if (!saveFileExists)
        {
            return null;
        }

        IEnumerable<T> lastGameData = GetLastNCompleteGamesData(1);
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
    /// Get data from the last n complete played games.
    /// </summary>
    /// <param name="nGames">Number of games to retrieve</param>
    public IEnumerable<T> GetLastNCompleteGamesData(int nGames)
    {
        List<T> lastCompleteGames = new List<T>();
        if (!saveFileExists)
        {
            return lastCompleteGames;
        }

        IEnumerable<string> csvLines = File.ReadLines(dataPath);
        string header = csvLines.First();
        int lineNo = csvLines.Count() - 1;

        // Start from end of file, and find n complete games
        while (lineNo > 0 && csvLines.Count() < nGames)
        {
            string line = File.ReadLines(dataPath).ElementAt(lineNo);
            T gameData = Activator.CreateInstance(typeof(T), CreateCsvLineDict(header, line)) as T;
            if (gameData.gameCompleted)
            {
                lastCompleteGames.Append(gameData);
            }
            lineNo--;
        }

        return lastCompleteGames;
    }

    private Dictionary<string, string> CreateCsvLineDict(string csvHeader, string csvRow)
    {
        string[] headerNames = csvHeader.Split(',');
        string[] values = csvRow.Split(",");
        Dictionary<string, string> headerToValue = new();

        for (int i = 0; i < headerNames.Length; i++)
        {
            headerToValue.Add(headerNames[i], values[i]);
        }
        return headerToValue;
    }

    private void CheckSaveFileExists()
    {
        if (File.Exists(dataPath))
        {
            saveFileExists = true;
        }
        else
        {
            saveFileExists = false;
        }
    }
}
