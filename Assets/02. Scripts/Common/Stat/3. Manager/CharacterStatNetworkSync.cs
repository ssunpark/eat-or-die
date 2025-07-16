using Fusion;

public class CharacterStatNetworkSync : NetworkBehaviour
{
    [Networked, Capacity(12)] public NetworkArray<float> NetStats => default;

    private StatManager _statManager;

    public void Initialize(StatManager manager)
    {
        _statManager = manager;
        if (HasStateAuthority)
            SyncAllStats();
    }

    public void SyncAllStats()
    {
        foreach (var kvp in _statManager.GetStatSnapshot())
        {
            int index = GetIndex(kvp.Key);
            if (index >= 0)
                NetStats.Set(index, kvp.Value);
        }
    }

    private static int GetIndex(EStatType type)
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
            EStatType.MaxSatiety => 10,
            EStatType.MaxHealth => 11,
            _ => -1
        };
    }
}