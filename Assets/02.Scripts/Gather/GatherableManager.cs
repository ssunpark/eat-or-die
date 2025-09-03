using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GatherableManager : BehaviourSingleton<GatherableManager>
{
    private const string GATHERABLE_CSV_PATH = "/GatherableCSV/GatherableDrop.csv";

    private Dictionary<int, List<GatherableDropData>> _dropTable = new();

    private void Awake()
    {
        LoadCsv();
        DontDestroyOnLoad(gameObject);
    }

    private void LoadCsv()
    {
        string fullPath = Application.streamingAssetsPath + GATHERABLE_CSV_PATH;
        var rows = CSVLoader<GatherableDropData>.LoadCSV(fullPath);
        _dropTable = rows
            .GroupBy(r => r.GatherableID)
            .ToDictionary(g => g.Key, g => g.ToList());
        Debug.Log($"[GatherableManager] Loaded {_dropTable.Count} gatherables from CSV");
    }

    public List<GatherableDropData> GetDrops(int gatherableId)
    {
        if (_dropTable.TryGetValue(gatherableId, out var list))
            return list;
        return new List<GatherableDropData>();
    }
}