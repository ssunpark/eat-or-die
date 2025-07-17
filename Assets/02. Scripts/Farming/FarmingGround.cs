using System;
using Fusion;
using UnityEngine;

public class FarmingGround : NetworkBehaviour
{
    // 땅에 심어진 작물에 대한 ID
    private int _seedID;

    [Networked]
    public EFarmingGroundState State { get; set; }

    public override void Spawned()
    {
        State = EFarmingGroundState.None;
    }

    public void Plow()
    {
        // 밭 갈기
        State = EFarmingGroundState.Plowed;
    }

    public void Water()
    {
        if (State == EFarmingGroundState.Plowed)
        {
            State = EFarmingGroundState.Watered;
        }
        else if (State == EFarmingGroundState.Planted)
        {
            Grow();
        }
    }

    public void Plant(int seedID)
    {
        _seedID = seedID;
        
        if (State == EFarmingGroundState.Plowed)
        {
            State = EFarmingGroundState.Planted;
        }
        else if (State == EFarmingGroundState.Watered)
        {
            Grow();
        }
    }

    private void Grow()
    {
        State = EFarmingGroundState.Growing;
        // 식물 자라남
        RPC_CreatePlantObject(_seedID);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_CreatePlantObject(int seedID)
    {
        if (!Runner.IsServer)
        {
            return;
        }

        if (!FarmingManager.Instance.SeedDictionary.TryGetValue(seedID, out PlantData plant))
        {
            throw new Exception("없는 아이템입니다.");
        }

        var plantObject = Runner.Spawn(FarmingManager.Instance.PlantObjectPrefab,
            inputAuthority: null,
            onBeforeSpawned: (runner, obj) =>
            {
                var plant = obj.GetComponent<PlantObject>();
                plant.PlantID = seedID;
                plant.GrowthLevel = 1;
            });
        plantObject.transform.SetParent(transform);
    }
}