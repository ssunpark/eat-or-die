using Fusion;
using UnityEngine;
using System;

public partial class PlayerStatNetwork : NetworkBehaviour
{
    private const int StatCount = 11; // MaxHealth 제외

    [Networked, Capacity(StatCount), OnChangedRender(nameof(OnStatChangedRender))]
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

    private float[] _lastValues = new float[StatCount];

    public override void FixedUpdateNetwork()
    {
        for (int i = 0; i < StatCount; i++)
        {
            float current = NetStats.Get(i);
            if (!Mathf.Approximately(current, _lastValues[i]))
            {
                _lastValues[i] = current;
                OnStatChanged(i, current);
            }
        }
    }

    private void OnStatChanged(int index, float newValue)
    {
        EStatType type = GetStatTypeFromIndex(index);
        Debug.Log($"[Client] {type} changed to {newValue}");
        // ➜ UI 반영, 이펙트 등 실행
    }

    private void OnStatChangedInternal()
    {
        Debug.Log("🎯 [Client] 스탯 값 변경 감지됨 → UI 등 반영");
        // 여기서 UI에 반영하거나 관련 로직 호출 가능
    }
}
