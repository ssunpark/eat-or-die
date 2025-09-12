/// 업적 진행도 (값 객체)
public class AchievementProgress
{
    public long Current { get; private set; }
    public long Target { get; private set; }

    public AchievementProgress(long current = 0, long target = 0)
    {
        Current = current;
        Target = target;
    }

    public void Set(long current, long target)
    {
        Current = current;
        Target = target;
    }

    public void SetCurrent(long v) => Current = v;
    public void SetTarget(long v) => Target = v;
}