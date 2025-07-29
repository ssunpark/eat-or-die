using Fusion;

public class CharacterStatNetworkSync : NetworkBehaviour
{
    [Networked, Capacity(24)] public NetworkArray<float> NetStats => default;

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
            EStatType.MoveSpeed => 0,
            EStatType.Acceleration => 1,
            EStatType.JumpPower => 2,
            EStatType.SprintingMultiplier => 3,

            EStatType.TotalDamage => 4,
            EStatType.MeleeDamage => 5,
            EStatType.MagicDamage => 6,
            EStatType.AttackSpeed => 7,
            EStatType.AttackRange => 8,
            EStatType.CritChance => 9,
            EStatType.CritDamageRatio => 10,
            EStatType.BossDamage => 11,

            EStatType.Defense => 12,
            EStatType.MeleeDefense => 13,
            EStatType.MagicDefense => 14,
            EStatType.BossDefense => 15,

            EStatType.MaxHunger => 16,
            EStatType.HungerConsumptionOverTime => 17,
            EStatType.HungerRecoveryOverTime => 18,
            EStatType.HungerConsumeReduction => 19,

            EStatType.MaxMana => 20,
            EStatType.ManaRecoveryOverTime => 21,

            EStatType.HarvestBonusChance => 22,
            EStatType.CookBonusChance => 23,

            _ => -1
        };
    }

}