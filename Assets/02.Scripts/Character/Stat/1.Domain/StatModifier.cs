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
