using System;
using System.Collections.Generic;

public static class AchievementFactory {
    public static List<Achievement> Create(List<AchievementRawData> raws) {
        var list = new List<Achievement>();
        foreach (var raw in raws) {
            var criteria = BuildCriteria(raw);
            var ach = new Achievement(
                id: raw.Id,
                title: raw.Title,
                description: raw.Description,
                category: raw.Category,
                hidden: raw.Hidden,
                criteria: criteria
            );
            list.Add(ach);
        }
        return list;
    }

    private static ICriteriaSpec BuildCriteria(AchievementRawData raw) {
        return raw.CriteriaType switch {
            ECriteriaType.CounterReach =>
                new CounterReachSpec(raw.CriteriaKey, raw.CriteriaTarget),
            ECriteriaType.OneShotEvent =>
                new OneShotEventSpec(raw.CriteriaKey),
            _ => throw new NotSupportedException($"Unsupported CriteriaType {raw.CriteriaType}")
        };
    }
}