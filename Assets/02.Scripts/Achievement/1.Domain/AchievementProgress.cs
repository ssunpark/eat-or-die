/// 업적 진행도 (값 객체)
public class AchievementProgress
{
    public double Current { get; private set; }
    public double Target { get; private set; }

    public AchievementProgress(double current = 0, double target = 0)
    {
        Current = current;
        Target = target;
    }

    public void Set(double current, double target)
    {
        Current = current;
        Target = target;
    }

    public void SetCurrent(double v) => Current = v;
    public void SetTarget(double v) => Target = v;
}