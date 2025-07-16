public class PlayerStatData
{
    public EStatType StatType;
    public float BaseAmount;
    public bool CanLevelUp;
    public float IncreaseAmount;

    public PlayerStatData(EStatType type, float baseAmount, bool canLevelUp, float increaseAmount)
    {
        StatType = type;
        BaseAmount = baseAmount;
        CanLevelUp = canLevelUp;
        IncreaseAmount = increaseAmount;
    }
}