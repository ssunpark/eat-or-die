using System;

public sealed class AchievementUnlocked
{
    public int PlayerId { get; }
    public int AchievementId { get; }
    public DateTime UnlockedAtUtc { get; }

    public AchievementUnlocked(int id, DateTime when)
    {
        AchievementId = id;
        UnlockedAtUtc = when;
    }
}