using System.Collections.Generic;
using Fusion;
using Redcode.Pools;
using UnityEngine;

public struct PlantPoolKey
{
    public readonly int Key;
    public readonly int Level;

    public PlantPoolKey(int key, int level)
    {
        Key = key;
        Level = level;
    }
}

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

    private Dictionary<PlantPoolKey, Pool<Transform>> _plantPoolDictionary;

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
        _plantPoolDictionary = new Dictionary<PlantPoolKey, Pool<Transform>>();
        var seedRawDataList =
            ItemDataLoader.LoadItemRawData<SeedRawData>(
                $"{Application.streamingAssetsPath}{ITEM_CSV_PATH}/SeedTestCSV.csv");
        foreach (var rawData in seedRawDataList)
        {
            var seedItemData = new ItemData(rawData.ID, rawData.Name, rawData.Description, rawData.MaxStack,
                rawData.IconPath);
            var seedData = new SeedData(seedItemData, rawData.HarvestItemID, rawData.GrowthTime);

            _seedDictionary.Add(rawData.ID, seedData);

            // 풀링
            GameObject poolContainer = new GameObject($"{rawData.ID}_Pool");
            poolContainer.transform.SetParent(transform);
            for (int level = 1; level <= seedData.GrowthMaxLevel; level++)
            {
                _plantPoolDictionary.Add(new PlantPoolKey(rawData.ID, level),
                    Pool.Create(seedData.PlantPrefabDictionary[level].transform, 10, poolContainer.transform));
            }
        }
    }

    public GameObject GetPlant(PlantPoolKey plantPoolKey)
    {
        return _plantPoolDictionary[plantPoolKey].Get().gameObject;
    }
}