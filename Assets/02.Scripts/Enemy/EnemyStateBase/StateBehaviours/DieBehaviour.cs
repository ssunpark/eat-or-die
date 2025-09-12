using Fusion;
using UnityEngine;

public class DieBehaviour : AEnemyStateBehaviour
{
    [SerializeField]
    private float _despawnTime = 2f;

    protected override void OnEnterState()
    {
        Machine.Context.Owner.AnimationState = EAnimationState.Die;

        // 경험치 주기
        Machine.Context.Target.PlayerFSM.RPC_GrantExpOrderWithAmount(
            Machine.Context.Target.Object.InputAuthority,
            "KillMonster",
            (int)Machine.Context.StatManager.GetStat(EStatType.EnemyHunger)
        );
        // 업적 추가 $"Monster.{Machine.Context.Owner.EnemyID}"
        AchievementManager.Instance.AddMetricAndReevaluateServer(
            Machine.Context.Target.Object.InputAuthority,
            "Monster",
            1);
    }

    protected override void OnFixedUpdate()
    {
        if (Machine.StateTime <= _despawnTime)
            return;

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
            ItemProxySpawner.Instance.RPC_CreateItemObject(
                id: drop1ID,
                quantity: Random.Range(1, drop1Quantity),
                durability: ItemManager.Instance.GetItem(drop1ID).ItemDefinition.MaxDurability,
                position: Machine.Context.Owner.transform.position,
                rotation: Quaternion.identity,
                pickableTime: 0.5f);
        }

        if (Random.value < drop2Rate)
        {
            ItemProxySpawner.Instance.RPC_CreateItemObject(
                id: drop2ID,
                quantity: Random.Range(1, drop2Quantity),
                durability: ItemManager.Instance.GetItem(drop2ID).ItemDefinition.MaxDurability,
                position: Machine.Context.Owner.transform.position,
                rotation: Quaternion.identity,
                pickableTime: 0.5f);
        }
    }
}