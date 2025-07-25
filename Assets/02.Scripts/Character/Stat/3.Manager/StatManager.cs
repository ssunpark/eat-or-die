using System.Collections.Generic;
using System.Linq;

public class StatManager
{
    private Dictionary<EStatType, Stat> _statDict;

    public StatManager(IStatDataRepository repo)
    {
        _statDict = repo.GetCharacterStatData().ToDictionary(
            data => data.StatType,
            data => new Stat(data.BaseAmount, data.CanLevelUp, data.IncreaseAmount)
        );
    }

    public void UpdateStats(float t)
    {
        foreach (var stat in _statDict)
        {
            stat.Value.UpdateModifiers(t);
        }
    }

    public float GetStat(EStatType type) =>
        _statDict.TryGetValue(type, out var stat) ? stat.TotalStat : 0f;

    public Stat GetRawStat(EStatType type) =>
        _statDict.TryGetValue(type, out var stat) ? stat : null;

    public void ApplyModifier(EStatType type, StatModifier mod)
    {
        if (_statDict.TryGetValue(type, out var stat))
            stat.AddModifier(mod);
    }

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