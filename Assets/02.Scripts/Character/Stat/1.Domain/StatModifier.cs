public enum EStatModifierType
{
    Add,
    Multiply,
    Percentage
}

public class StatModifier
{
    public EStatModifierType Type;
    public float Value;
    public object Source; // 장비/버프 등 출처 구분용
    public bool IsBuff; // 버프 여부 (false면 Duration 알빠노입니다)
    public float Duration; // 지속시간 (초 단위)

    public StatModifier(EStatModifierType type, float value, object source, bool isBuff = false, float duration = 0)
    {
        Type = type;
        Value = value;
        Source = source;
        IsBuff = isBuff;
        Duration = duration;
    }
}

public enum EModifierKey
{
    None = 0,

    Food_HungerRecoveryOverTime = 210001,
    Food_HungerConsumeReduction = 210002,
    Food_MaxHunger = 210003,
    Food_ManaRecoveryOverTime = 210004,
    Food_MaxMana = 210005,
    Food_MoveSpeed = 210006,
    Food_TotalDamage = 210007,
    Food_MeleeDamage = 210008,
    Food_MagicDamage = 210009,
    Food_AttackSpeed = 210010,
    Food_Defense = 210011,
    Food_MeleeDefense = 210012,
    Food_MagicDefense = 210013,
    Food_BossDamage = 210014,
    Food_BossDefense = 210015,
    Food_CritChance = 210016,
    Food_CritDamageRatio = 210017,
}

