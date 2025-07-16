using System.Collections.Generic;
using System.Linq;

public class PlayerStatManager
{
    private Dictionary<EStatType, Stat> _statDict;

    public PlayerStatManager(IStatDataRepository repo)
    {
        _statDict = repo.GetPlayerStatData().ToDictionary(
            data => data.StatType,
            data => new Stat(data.BaseAmount, data.CanLevelUp, data.IncreaseAmount)
        );
    }

    public float GetStat(EStatType type) =>
        _statDict.TryGetValue(type, out var stat) ? stat.TotalStat : 0f;

    public void ApplyModifier(EStatType type, StatModifier mod) =>
        _statDict[type].AddModifier(mod);

    public void RemoveModifiersFrom(object source)
    {
        foreach (var stat in _statDict.Values)
            stat.RemoveModifiersFrom(source);
    }

    public Dictionary<EStatType, float> GetStatSnapshot() =>
        _statDict.ToDictionary(x => x.Key, x => x.Value.TotalStat);


    public void ApplyBaseStats(Dictionary<EStatType, float> baseStats)
    {
        foreach (var kvp in baseStats)
        {
            if (_statDict.TryGetValue(kvp.Key, out var stat))
            {
                stat.SetBaseStat(kvp.Value);
            }
        }
    }
}
