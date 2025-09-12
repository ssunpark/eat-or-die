using System.Collections.Generic;

public class InMemoryAchievementCatalog : IAchievementCatalog {
    private readonly Dictionary<int, Achievement> _map;
    private readonly Achievement[] _all;

    public InMemoryAchievementCatalog(IEnumerable<Achievement> list) {
        _map = new();
        var tmp = new List<Achievement>();
        foreach (var a in list) {
            if (_map.ContainsKey(a.Id)) throw new System.Exception($"Duplicate Achievement ID: {a.Id}");
            _map[a.Id] = a;
            tmp.Add(a);
        }
        _all = tmp.ToArray();
    }

    public IReadOnlyList<Achievement> GetAll() => _all;
    public bool TryGet(int id, out Achievement ach) => _map.TryGetValue(id, out ach!);
}