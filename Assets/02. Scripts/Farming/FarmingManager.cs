using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FarmingManager : NetworkBehaviour
{
    private const string ITEM_CSV_PATH = "/ItemCSV";
    // 작물 데이터 관리
    public static FarmingManager Instance { get; private set; }
    
    [SerializeField]
    private NetworkPrefabRef _plantObjectPrefab;
    
    public NetworkPrefabRef PlantObjectPrefab => _plantObjectPrefab;

    private Dictionary<int, SeedData> _seedDictionary;
    public IReadOnlyDictionary<int, SeedData> SeedDictionary => _seedDictionary;

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Runner.Despawn(Object); // 중복 방지
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _seedDictionary = new Dictionary<int, SeedData>();
        var plantRawDataList =
            ItemDataLoader.LoadItemRawData<SeedRawData>($"{Application.streamingAssetsPath}{ITEM_CSV_PATH}/SeedTestCSV.csv");
        foreach (var rawData in plantRawDataList)
        {
            var plantItemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.MaxStack,
                rawData.IconPath);
            var plantData = new SeedData(plantItemData, rawData.HarvestItemID, rawData.GrowthTime);
            _seedDictionary.Add(rawData.ID, plantData);
        }
    }
}