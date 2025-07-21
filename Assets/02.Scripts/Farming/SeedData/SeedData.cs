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
    public readonly int HarvestItemID;
    public readonly float GrowthTime;
    public readonly bool IsRandomSeed;

    private Dictionary<int, GameObject> _plantPrefabDictionary;
    public IReadOnlyDictionary<int, GameObject> PlantPrefabDictionary => _plantPrefabDictionary;

    public SeedData(SeedRawData rawData)
    {
        ID = rawData.ID;

        // 랜덤 씨드
        if (rawData.HarvestItemID == null)
        {
            IsRandomSeed = true;
            return;
        }
        
        HarvestItemID = rawData.HarvestItemID ?? 0;
        GrowthTime = rawData.GrowthTime;
        
        MaxGrowthLevel = MAX_GROWTHLEVEL;
        DriedTime = DRIEDTIME;
        
        string addressablePath = rawData.AddressablePath;
        _plantPrefabDictionary = new Dictionary<int, GameObject>();
        for (int level = 1; level <= MaxGrowthLevel; level++)
        {
            int levelID = level;
            string addressableAssetName = level != MaxGrowthLevel
                ? $"{addressablePath}{level} Variant"
                : $"{addressablePath}Dried Variant";
            Addressables.LoadAssetAsync<GameObject>(addressableAssetName).Completed +=
                (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        _plantPrefabDictionary.Add(levelID, handle.Result);
                    }
                    else
                    {
                        throw new Exception("프리팹 로드에 실패했습니다.");
                    }
                };
            
        }
    }
}