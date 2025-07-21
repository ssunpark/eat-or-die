using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using UnityEngine;

public class CSVLoader<T>
{
    public static List<T> LoadFoodCSV(string path)
    {
        if (!File.Exists(path))
        {
            UnityEngine.Debug.LogError($"CSV 파일 없음: {path}");
            return new List<T>();
        }

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<T>().ToList();

        return records;
    }
}