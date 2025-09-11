using System;

/// 합성 규칙(AND)
public class AndSpec : ICriteriaSpec
{
    private readonly ICriteriaSpec[] _children;

    public AndSpec(params ICriteriaSpec[] children)
    {
        _children = children;
    }

    public void Accumulate(AchievementProgress p, AchievementEvent e, Func<string, long> getStat)
    {
        foreach (var c in _children)
            c.Accumulate(p, e, getStat);
        // AND는 p.Current/Target을 단일 값으로 표현하기 애매 -> UI 전용 Progress는 별도 구성 가능
    }

    public bool IsSatisfied(AchievementProgress p)
    {
        foreach (var c in _children)
            if (!c.IsSatisfied(p))
                return false;
        return true;
    }
}