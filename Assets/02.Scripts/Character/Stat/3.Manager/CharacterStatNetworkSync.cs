using Fusion;

public class CharacterStatNetworkSync : NetworkBehaviour
{
    [Networked, Capacity(23)] public NetworkArray<float> NetStats => default;

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
            EStatType.ConsumptionOverTime => 0,
            EStatType.HungerRecoveryOverTime => 1,
            EStatType.MoveSpeed => 2,
            EStatType.Acceleration => 3,
            EStatType.JumpPower => 4,
            EStatType.MeleeDamage => 5,
            EStatType.MagicDamage => 6,
            EStatType.AttackSpeed => 7,
            EStatType.CritChance => 8,
            EStatType.Defense => 9,
            EStatType.SprintingMultiplier => 10,
            EStatType.MaxHunger => 11,
            EStatType.AttackRange => 12,
            EStatType.HarvestBonusChance => 13,
            EStatType.CookBonusChnace => 14,
            EStatType.MaxMana => 15,
            EStatType.ManaRecoveryOverTime => 16,
            EStatType.CritDamageRatio => 17,
            EStatType.MeleeDefense => 18,
            EStatType.MagicDefense => 19,
            EStatType.BossDamage => 20,
            EStatType.BossDefense => 21,
            EStatType.HungerConsumeReduction => 22,
            _ => -1
        };
    }
}