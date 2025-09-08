using System;

/// 단순 카운터: 특정 통계 키가 목표 이상이면 달성
public class CounterReachSpec : ICriteriaSpec {
    private readonly string _statKey;     // 예: "kills.total", "currency.wallet"
    private readonly long _target;

    public CounterReachSpec(string statKey, long target) {
        _statKey = statKey; _target = target;
    }

    public void Accumulate(AchievementProgress p, AchievementEvent e, Func<string, long> getStat) {
        var cur = getStat(_statKey);
        p.Set(cur, _target);
    }

    public bool IsSatisfied(AchievementProgress p) => p.Current >= p.Target;
}