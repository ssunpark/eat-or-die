[System.Serializable]
public class SerializableStatEntry
{
    public EStatType StatType;
    public Stat Stat;
    public float TotalStat;

    public SerializableStatEntry(EStatType type, Stat stat)
    {
        StatType = type;
        Stat = stat;
        TotalStat = stat.TotalStat;
    }
}