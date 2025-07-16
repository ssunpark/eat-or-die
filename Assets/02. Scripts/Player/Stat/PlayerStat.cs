using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStat : MonoBehaviour
{
    public Dictionary<EStatType, Stat> StatDictionary = new Dictionary<EStatType, Stat>();

    [SerializeField] private List<SerializableStatEntry> _statList = new(); // 인스펙터에서 보기용
    public Action OnDictionaryLoaded;

    private void Awake()
    {
        LoadData();
    }
    private void LoadData()
    {
        //var data = DataTable.Instance.GetPlayerStatDataList();
        var data = MockStatDataTable.GetMockData();
        StatDictionary.Clear();
        _statList.Clear();

        foreach (var statData in data)
        {
            var stat = new Stat(statData.BaseAmount, statData.CanLevelUp, statData.IncreaseAmount);
            StatDictionary[statData.StatType] = stat;
            _statList.Add(new SerializableStatEntry(statData.StatType, stat));
        }

        OnDictionaryLoaded?.Invoke();
       // UIEventManager.Instance.OnDisplayStatChanged?.Invoke(new StatSnapshot());
    }
    public void ApplyBaseStats(Dictionary<EStatType, float> baseStats)
    {
        foreach (var kvp in baseStats)
        {
            if (StatDictionary.TryGetValue(kvp.Key, out var stat))
            {
                stat.SetBaseStat(kvp.Value);
            }
        }
    }
    public float GetStat(EStatType type)
    {
        if (StatDictionary.TryGetValue(type, out var stat))
        {
            return stat.TotalStat;
        }

        Debug.LogWarning($"Stat not found: {type}");
        return 0f;
    }

    public void ApplyModifier(EStatType statType, StatModifier modifier)
    {
        if (StatDictionary.TryGetValue(statType, out var stat))
            stat.AddModifier(modifier);
    }

    public void RemoveModifiersFromSource(object source)
    {
        foreach (var stat in StatDictionary.Values)
            stat.RemoveModifiersFrom(source);
    }
}
