using System.Collections.Generic;

public class LocalAchievementProgressQuery : IAchievementProgressQuery
{
    private readonly Dictionary<string, long> _vals = new();

    public long GetValue(string key) =>
        _vals.TryGetValue(key, out var v) ? v : 0;

    public void Set(string key, long value) => _vals[key] = value; // 헬퍼
}