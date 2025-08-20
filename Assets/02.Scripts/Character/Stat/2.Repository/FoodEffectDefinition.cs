using System.Collections.Generic;
public sealed class FoodEffectDefinition
{
    public EStatType StatType;
    public EStatModifierType Op; // Add / Multiply
    public string Desc;
    public string Extra;
}

public sealed class FoodEffectDB
{
    private static FoodEffectDB _inst;
    public static FoodEffectDB Instance => _inst ??= new FoodEffectDB();

    private readonly Dictionary<EStatType, FoodEffectDefinition> _byStat = new();
    public bool IsLoaded { get; private set; }

    public FoodEffectDefinition Get(EStatType stat) =>
        _byStat.TryGetValue(stat, out var d) ? d : null;

    public void LoadEffects(string effectCsvPath)
    {
        if (IsLoaded) return;
        var rows = CSVLoader<FoodEffectRow>.LoadCSV(effectCsvPath);
        _byStat.Clear();
        foreach (var r in rows)
        {
            _byStat[r.StatType] = new FoodEffectDefinition
            {
                StatType = r.StatType,
                Op = r.Op,
                Desc = r.Description,
                Extra = r.ExtraDescription
            };
        }
        IsLoaded = true;
    }
}

public struct FoodEffectEntry
{
    public EStatType Stat;
    public EStatModifierType Op;
    public float Value;
    public float Duration;
}

public sealed class FoodDB
{
    private static FoodDB _inst;
    public static FoodDB Instance => _inst ??= new FoodDB();

    private readonly Dictionary<int, List<FoodEffectEntry>> _map = new();
    public bool IsLoaded { get; private set; }

    public IReadOnlyList<FoodEffectEntry> Get(int foodId) =>
        _map.TryGetValue(foodId, out var list) ? list : null;

    public void LoadFoods(string path)
    {
        if (IsLoaded) return;
        var rows = CSVLoader<FoodRow>.LoadCSV(path);
        _map.Clear();

        foreach (var r in rows)
        {
            var list = new List<FoodEffectEntry>(3);

            void Add(EStatType stat, float val, float dur)
            {
                if (stat == default) return;
                var def = FoodEffectDB.Instance.Get(stat);
                if (def == null) { UnityEngine.Debug.LogWarning($"Food effect def missing: {stat}"); return; }

                var op = def.Op;
                float normalized = (op == EStatModifierType.Multiply && val > 1f) ? val * 0.01f : val;

                list.Add(new FoodEffectEntry { Stat = stat, Op = op, Value = normalized, Duration = dur });
            }

            if (r.EffectStat1.HasValue)
            {
                Add(r.EffectStat1.Value, r.Value1.Value, r.Duration1.Value);
            }
            if(r.EffectStat2.HasValue)
            {
                Add(r.EffectStat2.Value, r.Value2.Value, r.Duration2.Value);
            }
            if (r.EffectStat3.HasValue)
            {
                Add(r.EffectStat3.Value, r.Value3.Value, r.Duration3.Value);
            }

            _map[r.FoodId] = list;
        }
        IsLoaded = true;
    }

    private static float NormalizeValueIfNeeded(EStatModifierType op, float raw)
    {
        if (op == EStatModifierType.Multiply && raw > 1f)
            return raw * 0.01f;
        return raw;
    }
}