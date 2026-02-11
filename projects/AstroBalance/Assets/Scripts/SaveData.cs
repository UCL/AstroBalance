using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
                sw.WriteLine(GameDataToCsv(gameData, true));
                saveFileExists = true;
            }

            sw.WriteLine(GameDataToCsv(gameData, false));
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
        while (lineNo > 0 && lastCompleteGames.Count() < nGames)
        {
            string line = File.ReadLines(dataPath).ElementAt(lineNo);

            T gameData = CsvToGameData(header, line);
            if (gameData.gameCompleted)
            {
                lastCompleteGames.Append(gameData);
            }
            lineNo--;
        }

        return lastCompleteGames;
    }

    private T CsvToGameData(string csvHeader, string csvRow)
    {
        string[] headerNames = csvHeader.Split(',');
        string[] values = csvRow.Split(",");

        T gameData = new();
        for (int i = 0; i < headerNames.Length; i++)
        {
            FieldInfo field = typeof(T).GetField(headerNames[i]);
            field.SetValue(gameData, Convert.ChangeType(values[i], field.FieldType));
        }

        return gameData;
    }

    private string GameDataToCsv(T gameData, bool header)
    {
        StringBuilder csvString = new StringBuilder();
        FieldInfo[] fields = GetFields(gameData);

        for (int i = 0; i < fields.Length; i++)
        {
            if (header)
            {
                csvString.Append(fields[i].Name);
            }
            else
            {
                csvString.Append(fields[i].GetValue(gameData));
            }
            if (i < fields.Length - 1)
            {
                csvString.Append(",");
            }
        }

        return csvString.ToString();
    }

    /// <summary>
    /// Return info on all public fields.
    /// Order is: date, startTime, endTime, gameCompleted, then any
    /// other fields in alphabetical order.
    /// </summary>
    private FieldInfo[] GetFields(T gameData)
    {
        Type type = gameData.GetType();
        FieldInfo[] fields = type.GetFields();
        FieldInfo[] sortedFields = new FieldInfo[fields.Length];

        // We return date, startTime, endTime, gameCompleted first (as this is general data
        // for all games, and useful to have at the start of the csv)
        sortedFields[0] = type.GetField("date");
        sortedFields[1] = type.GetField("startTime");
        sortedFields[2] = type.GetField("endTime");
        sortedFields[3] = type.GetField("gameCompleted");

        // Then, all other fields sorted in alphabetical order
        Array.Sort(fields, (x, y) => String.Compare(x.Name, y.Name));

        int nextIndex = 4;
        foreach (FieldInfo field in fields)
        {
            if (!sortedFields.Contains(field))
            {
                sortedFields[nextIndex] = field;
                nextIndex++;
            }
        }

        return sortedFields;
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
