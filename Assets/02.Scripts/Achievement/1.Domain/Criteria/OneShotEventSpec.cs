using System;

/// 즉시형: 특정 이벤트가 오면 단번에 충족(예: Boss 처치 1회)
public class OneShotEventSpec : ICriteriaSpec {
    private readonly string _eventKey;

    public OneShotEventSpec(string eventKey) { _eventKey = eventKey; }

    public void Accumulate(AchievementProgress p, AchievementEvent e, Func<string, long> _) {
        if (e.Key == _eventKey) { p.SetCurrent(1); p.SetTarget(1); }
    }

    public bool IsSatisfied(AchievementProgress p) => p.Current >= p.Target && p.Target > 0;
}