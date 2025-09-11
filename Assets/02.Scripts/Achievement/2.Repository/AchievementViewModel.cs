using System;

public class AchievementViewModel
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }
    public long Current { get; private set; }
    public long Target { get; private set; }
    public bool IsUnlocked { get; private set; }
    public DateTime? UnlockedAtUtc { get; private set; }

    public static AchievementViewModel From(Achievement ach, PlayerAchievement pa) => new()
    {
        Id = pa.AchievementId,
        Title = ach.Title,
        Description = ach.Description,
        Category = ach.Category,
        Current = pa.Progress.Current,
        Target = pa.Progress.Target,
        IsUnlocked = pa.IsUnlocked,
        UnlockedAtUtc = pa.UnlockedAtUtc
    };
}