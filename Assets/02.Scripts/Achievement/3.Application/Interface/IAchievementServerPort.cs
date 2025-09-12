using Fusion;

public interface IAchievementServerPort
{
    public void HandleEventServer(PlayerRef player, AchievementEvent e);
    public void AddMetricAndReevaluateServer(PlayerRef player, string key, long delta, bool emitToasts = true);
}