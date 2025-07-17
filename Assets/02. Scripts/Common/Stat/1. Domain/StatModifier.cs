public enum StatModifierType
{
    Add,
    Multiply,
    Percentage
}

public class StatModifier
{
    public StatModifierType Type;
    public float Value;
    public object Source; // 장비/버프 등 출처 구분용

    public StatModifier(StatModifierType type, float value, object source)
    {
        Type = type;
        Value = value;
        Source = source;
    }
}
