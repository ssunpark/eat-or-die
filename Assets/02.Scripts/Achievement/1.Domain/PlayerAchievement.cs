using System;

/// 플레이어별 업적 상태 (Aggregate)
public class PlayerAchievement {
    public int AchievementId { get; }
    public AchievementProgress Progress { get; } = new AchievementProgress();
    public bool IsUnlocked { get; private set; }
    public DateTime? UnlockedAtUtc { get; private set; }

    public PlayerAchievement(int id) {
        AchievementId = id;
    }

    public void Unlock(DateTime utcNow) {
        if (IsUnlocked) return;
        IsUnlocked = true;
        UnlockedAtUtc = utcNow;
    }
}