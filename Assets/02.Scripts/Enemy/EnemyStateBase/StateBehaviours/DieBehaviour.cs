using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;

public class DieBehaviour : AEnemyStateBehaviour
{
    [SerializeField] private float _despawnTime = 2f;

    protected override void OnEnterState()
    {
        Machine.Context.Owner.AnimationState = EAnimationState.Die;
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime <= _despawnTime) return;
        
        DropItems();
        NetworkObject owner = GetComponentInParent<NetworkObject>();
        Runner.Despawn(owner);
    }

    protected override bool CanExitState(AEnemyStateBehaviour nextState)
    {
        return false;
    }

    private void DropItems()
    {
        float drop1Rate = EnemyDataManager.Instance.EnemyRawDataDictionary[Machine.Context.Owner.EnemyID]
            .DropItem1Rate;
        float drop2Rate = EnemyDataManager.Instance.EnemyRawDataDictionary[Machine.Context.Owner.EnemyID]
            .DropItem2Rate;
        int drop1ID = EnemyDataManager.Instance.EnemyRawDataDictionary[Machine.Context.Owner.EnemyID]
            .DropItem1ID;
        int drop2ID = EnemyDataManager.Instance.EnemyRawDataDictionary[Machine.Context.Owner.EnemyID]
            .DropItem2ID;
        int drop1Quantity = EnemyDataManager.Instance.EnemyRawDataDictionary[Machine.Context.Owner.EnemyID]
            .DropItem1Count;
        int drop2Quantity = EnemyDataManager.Instance.EnemyRawDataDictionary[Machine.Context.Owner.EnemyID]
            .DropItem2Count;
        
        if (Random.value < drop1Rate)
        {
            ItemManager.Instance.RPC_CreateItemObject(
                drop1ID,
                Random.Range(1, drop1Quantity),
                ItemManager.Instance.GetItem(drop1ID).ItemDefinition.MaxDurability,
                Machine.Context.Owner.transform.position,
                Quaternion.identity);
        }
        
        if (Random.value < drop2Rate)
        {
            ItemManager.Instance.RPC_CreateItemObject(
                drop2ID,
                Random.Range(1, drop2Quantity),
                ItemManager.Instance.GetItem(drop2ID).ItemDefinition.MaxDurability,
                Machine.Context.Owner.transform.position,
                Quaternion.identity);
        }
    }
}
