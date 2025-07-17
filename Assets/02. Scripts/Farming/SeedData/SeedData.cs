using System;
using System.Collections.Generic;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SeedData
{
    public readonly int GrowthMaxLevel = 6;
    public readonly ItemData ItemData;
    public readonly int HarvestID;
    public readonly float GrowthTime;
    
    private Dictionary<int, GameObject> _plantPrefabDictionary;
    public IReadOnlyDictionary<int, GameObject> PlantPrefabDictionary => _plantPrefabDictionary;

    public SeedData(ItemData itemData, int? harvestID, float growthTime)
    {
        ItemData = itemData;
        HarvestID = harvestID ?? 0;
        GrowthTime = growthTime;

        _plantPrefabDictionary = new Dictionary<int, GameObject>();
        for (int level = 1; level <= GrowthMaxLevel; level++)
        {
            int levelID = level;
            string addressableAssetName = level != GrowthMaxLevel ? $"SFF_Potato_Crop_{level} Variant" : "SFF_Potato_Crop_Dried Variant";
            // 풀링을 위해 동기로 다 로드
            GameObject plantPrefab = Addressables.LoadAssetAsync<GameObject>(addressableAssetName).WaitForCompletion();
            _plantPrefabDictionary.Add(levelID, plantPrefab);
        }
    }
}