using System;

/// 업적 평가 규칙 인터페이스 (순수 도메인)
public interface ICriteriaSpec {
    public string StatKey { get; }
    public long Target { get; }
    // 이벤트 반영 -> 진행도 갱신
    public void Accumulate(AchievementProgress p, AchievementEvent e, Func<string, long> getStat);
    // 달성 판정(진행도를 바탕으로) 
    public bool IsSatisfied(AchievementProgress p);
}