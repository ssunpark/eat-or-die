using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FarmingManager : NetworkBehaviour
{
    // 작물 데이터 관리
    public static FarmingManager Instance { get; private set; }
    
    [SerializeField]
    private NetworkPrefabRef _plantObjectPrefab;
    
    public NetworkPrefabRef PlantObjectPrefab => _plantObjectPrefab;

    private Dictionary<int, PlantData> _seedDictionary;
    public IReadOnlyDictionary<int, PlantData> SeedDictionary => _seedDictionary;

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

        Init();
    }

    private void Init()
    {
        _seedDictionary = new Dictionary<int, PlantData>();
        var plantRawDataList =
            ItemDataLoader.LoadItemRawData<PlantRawData>($"{Application.streamingAssetsPath}/SeedTestData.csv");
        foreach (var rawData in plantRawDataList)
        {
            var plantItemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.MaxStack,
                rawData.IconPath);
            var plantData = new PlantData(plantItemData, rawData.HarvestItemID, rawData.GrowthTime);
            _seedDictionary.Add(rawData.ID, plantData);
        }
    }
}