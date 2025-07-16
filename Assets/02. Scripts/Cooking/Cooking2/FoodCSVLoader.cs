using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public class FoodCSVLoader : MonoBehaviour
{
    public static FoodCSVLoader Instance { get; private set; }

    public List<FoodCSVData> AllDataList = new List<FoodCSVData>();
    public List<FoodCSVData> HarvestDataList = new List<FoodCSVData>();
    public List<FoodCSVData> BuffDataList = new List<FoodCSVData>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCSV("FoodCSV/FoodCSV2.csv");
    }
    
    public void LoadCSV(string fileName)
    {
        AllDataList.Clear();
        HarvestDataList.Clear();
        BuffDataList.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"CSV 파일이 존재하지 않습니다: {path}");
            return;
        }

        using (StreamReader reader = new StreamReader(path))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<FoodCSVData>();
            foreach (var record in records)
            {
                AllDataList.Add(record);

                if (record.ERecipeType == "Harvest")
                {
                    HarvestDataList.Add(record);
                }
                else if (record.ERecipeType == "Buff")
                {
                    BuffDataList.Add(record);
                }
            }
        }

        Debug.Log($"로드 완료: 총 {AllDataList.Count}개, Harvest {HarvestDataList.Count}개, Buff {BuffDataList.Count}개");
    }
}
