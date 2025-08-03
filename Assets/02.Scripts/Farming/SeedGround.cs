using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkObject))]
public class SeedGround : NetworkBehaviour
{
    [Networked]
    public bool IsPlanted { get; set; }
    
    private FarmingGround _parentFarmingGround;

    public override void Spawned()
    {
        _parentFarmingGround = GetComponentInParent<FarmingGround>();
    }

    public void Plant(int seedID)
    {
        if (_parentFarmingGround.State == EFarmingGroundState.None)
        {
            return;
        }
        
        RPC_CreatePlantObject(seedID);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_CreatePlantObject(int seedID)
    {
        if (!Runner.IsServer)
        {
            return;
        }

        if (IsPlanted)
        {
            return;
        }

        if (!FarmingManager.Instance.TryGetSeedData(seedID, out SeedData seedData))
        {
            throw new Exception("없는 작물입니다.");
        }
        
        int finalSeedID = seedData.IsRandomSeed ? FarmingManager.Instance.GetRandomSeedID(seedID) : seedID;

        var plantObject = Runner.Spawn(FarmingManager.Instance.PlantObjectPrefab,
            inputAuthority: null,
            onBeforeSpawned: (runner, obj) =>
            {
                var plant = obj.GetComponent<PlantObject>();
                plant.PlantID = finalSeedID;
                plant.GrowthLevel = 1;
                plant.transform.SetParent(transform);
                plant.transform.localPosition = Vector3.zero;
            });

        IsPlanted = true;
    }
}