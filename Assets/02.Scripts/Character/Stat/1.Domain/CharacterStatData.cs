public class CharacterStatData
{
    public EStatType StatType;
    public float BaseAmount;

    public CharacterStatData(EStatType type, float baseAmount)
    {
        StatType = type;
        BaseAmount = baseAmount;
    }
}