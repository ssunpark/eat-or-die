using System;
using System.Collections.Generic;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SeedData
{
    private const int MAX_GROWTHLEVEL = 6;
    private const int DRIEDTIME = 600;
    
    public readonly int MaxGrowthLevel;
    public readonly int DriedTime;
    public readonly int ID;
    public readonly string AddressablePath;
    public readonly int HarvestItemID;
    public readonly float GrowthTime;

    private Dictionary<int, GameObject> _plantPrefabDictionary;
    public IReadOnlyDictionary<int, GameObject> PlantPrefabDictionary => _plantPrefabDictionary;

    public SeedData(SeedRawData rawData)
    {
        ID = rawData.ID;
        AddressablePath = rawData.AddressablePath;
        // TODO: 랜덤 수확물 처리 필요
        HarvestItemID = rawData.HarvestItemID ?? 0;
        GrowthTime = rawData.GrowthTime;
        
        MaxGrowthLevel = MAX_GROWTHLEVEL;
        DriedTime = DRIEDTIME;

        // TODO: 정식 경로로 수정 필요
        _plantPrefabDictionary = new Dictionary<int, GameObject>();
        for (int level = 1; level <= MaxGrowthLevel; level++)
        {
            int levelID = level;
            string addressableAssetName = level != MaxGrowthLevel
                ? $"SFF_Potato_Crop_{level} Variant"
                : "SFF_Potato_Crop_Dried Variant";
            // 풀링을 위해 동기로 다 로드
            GameObject plantPrefab = Addressables.LoadAssetAsync<GameObject>(addressableAssetName).WaitForCompletion();
            _plantPrefabDictionary.Add(levelID, plantPrefab);
        }
    }
}