using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

/// <summary>
/// Class to save / load data from a csv file.
/// </summary>
/// <typeparam name="T">The type of data</typeparam>
[System.Serializable]
public class SaveData<T>
    where T : Data, new()
{
    protected bool saveFileExists = false;
    protected string dataPath;

    /// <summary>
    /// Create a new Save Data collection. If available, this will
    /// build on previous save data from filename.csv
    /// </summary>
    /// <param name="filename">The filename of the save file</param>
    public SaveData(string filename)
    {
        // currently defaults to "C:\Users\username\AppData\LocalLow\DefaultCompany\AstroBalance" on Windows
        dataPath = Path.Combine(Application.persistentDataPath, filename + ".csv");
        CheckSaveFileExists();
    }

    /// <summary>
    /// Add data to the save file.
    /// </summary>
    /// <param name="data">Data from this session</param>
    public void Save(T data)
    {
        using (StreamWriter sw = new StreamWriter(dataPath, true))
        {
            if (!saveFileExists)
            {
                sw.WriteLine(DataToCsv(data, true));
                saveFileExists = true;
            }

            sw.WriteLine(DataToCsv(data, false));
        }
    }

    /// <summary>
    /// Overwrite the last (most recent) item in the save file.
    /// </summary>
    /// <param name="data">Data to overwrite with</param>
    public void Overwrite(T data)
    {
        if (!saveFileExists)
        {
            throw new InvalidOperationException("Can't overwrite file that doesn't exit");
        }

        List<string> csvLines = File.ReadLines(dataPath).ToList();
        csvLines[csvLines.Count() - 1] = DataToCsv(data, false);
        File.WriteAllLines(dataPath, csvLines);
    }

    /// <summary>
    /// Get most recent saved data item (i.e. the last row from the csv file).
    /// </summary>
    public T GetLast()
    {
        if (!saveFileExists)
        {
            return null;
        }

        IEnumerable<T> last = GetLastN(1);
        if (last.Count() != 1)
        {
            return null;
        }
        else
        {
            return last.ElementAt(0);
        }
    }

    /// <summary>
    /// Get a list of the last n saved data items (or as many as are available).
    /// Data is stored in chronological order, from earliest to latest (most recent data in final position).
    /// </summary>
    /// <param name="nData">Maximum number of data items to retrieve</param>
    public IEnumerable<T> GetLastN(int nData)
    {
        List<T> lastN = new List<T>();
        if (!saveFileExists)
        {
            return lastN;
        }

        IEnumerable<string> csvLines = File.ReadLines(dataPath);
        string header = csvLines.First();
        int lineNo = csvLines.Count() - 1;

        // Start from end of file, and read last n lines
        while (lineNo > 0 && lastN.Count() < nData)
        {
            string line = csvLines.ElementAt(lineNo);

            T data = CsvToData(header, line);
            lastN.Add(data);
            lineNo--;
        }
        lastN.Reverse();

        return lastN;
    }

    /// <summary>
    /// Convert csv header / row into a Data object.
    /// </summary>
    /// <param name="csvHeader">Csv header as string (first line of csv file)</param>
    /// <param name="csvRow">Csv row as string</param>
    /// <returns>Data object with fields populated by row values</returns>
    protected T CsvToData(string csvHeader, string csvRow)
    {
        string[] headerNames = csvHeader.Split(',');
        string[] values = csvRow.Split(",");

        T data = new();
        for (int i = 0; i < headerNames.Length; i++)
        {
            FieldInfo field = typeof(T).GetField(headerNames[i]);
            field.SetValue(data, Convert.ChangeType(values[i], field.FieldType));
        }

        return data;
    }

    /// <summary>
    /// Convert Data object to a csv string.
    /// </summary>
    /// <param name="data">Data to convert</param>
    /// <param name="headerOnly">When true, returns a csv header string (names of fields),
    /// otherwise returns a csv row string (values of fields)</param>
    /// <returns>Csv string</returns>
    private string DataToCsv(T data, bool headerOnly)
    {
        StringBuilder csvString = new StringBuilder();
        FieldInfo[] fields = GetFields(data);

        for (int i = 0; i < fields.Length; i++)
        {
            if (headerOnly)
            {
                csvString.Append(fields[i].Name);
            }
            else
            {
                csvString.Append(fields[i].GetValue(data));
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
    /// Order is: date, startTime, endTime, then any
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

        // Then, all other fields sorted in alphabetical order
        Array.Sort(fields, (x, y) => String.Compare(x.Name, y.Name));

        int nextIndex = 3;
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
