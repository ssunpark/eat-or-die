using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SeedData
{
    private const int GrowthMaxLevel = 6;
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
            Addressables.LoadAssetAsync<GameObject>(addressableAssetName).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _plantPrefabDictionary.Add(levelID, handle.Result);
                }
                else
                {
                    throw new Exception("작물 로드에 실패했습니다.");
                }
            };
        }
    }
}