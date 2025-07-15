using Fusion;
using UnityEngine;
using System;

public partial class PlayerStatNetwork : NetworkBehaviour
{
    private const int StatCount = 11; // MaxHealth 제외

    [Networked, Capacity(StatCount)]
    public NetworkArray<float> NetStats => default;

    private PlayerStat _playerStat;

    public override void Spawned()
    {
        _playerStat = GetComponent<PlayerStat>();

        if (HasStateAuthority)
            UpdateAllStatNetworkValues();
    }

    public void UpdateAllStatNetworkValues()
    {
        if (_playerStat == null) return;

        foreach (EStatType type in Enum.GetValues(typeof(EStatType)))
        {
            if (type == EStatType.MaxHealth) continue;

            int index = GetStatIndex(type);
            if (index >= 0 && index < StatCount)
                NetStats.Set(index, _playerStat.GetStat(type));
        }
    }

    public float GetSyncedStat(EStatType type)
    {
        int index = GetStatIndex(type);
        return (index >= 0 && index < StatCount) ? NetStats.Get(index) : 0f;
    }

    private static int GetStatIndex(EStatType type)
    {
        return type switch
        {
            EStatType.ConsumptionRate => 0,
            EStatType.MaxShield => 1,
            EStatType.MoveSpeed => 2,
            EStatType.Acceleration => 3,
            EStatType.JumpPower => 4,
            EStatType.Damage => 5,
            EStatType.AttackSpeed => 6,
            EStatType.CritChance => 7,
            EStatType.Armor => 8,
            EStatType.SprintingMultiplier => 9,
            EStatType.Satiety => 10,
            _ => -1
        };
    }
}
