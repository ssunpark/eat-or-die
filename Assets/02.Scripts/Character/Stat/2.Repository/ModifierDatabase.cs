using System;
using System.Collections.Generic;
using UnityEngine;

public class ModifierDatabase
{
    private static ModifierDatabase _instance;
    public static ModifierDatabase Instance => _instance ??= new ModifierDatabase();

    private readonly Dictionary<int, ModifierDefinition> _map = new();
    public bool IsLoaded { get; private set; }

    public ModifierDefinition Get(int tid) =>
        _map.TryGetValue(tid, out var definition) ? definition : null;

    public void LoadFromCsvPath(string csvPath)
    {
        if(IsLoaded) return;
        var rows = CSVLoader<ModifierDefinition>.LoadCSV(csvPath);
        _map.Clear();
        foreach(var r in rows)
        {
            _map[r.TID] = new ModifierDefinition
            {
                TID = r.TID,
                StatType = r.StatType,
                StatModifierType = r.StatModifierType,
                Value = r.Value,
                DurationSec = r.DurationSec,
                Description = r.Description
            };
        }
        IsLoaded = true;
    }
}

