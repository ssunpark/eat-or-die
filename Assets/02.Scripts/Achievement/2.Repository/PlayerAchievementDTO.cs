using System;
using Firebase.Firestore;

[FirestoreData]
public class PlayerAchievementDTO
{
    [FirestoreProperty]
    public int AchievementId { get; set; }
    [FirestoreProperty]
    public double Current { get; set; }
    [FirestoreProperty]
    public bool IsUnlocked { get; set; }
    [FirestoreProperty]
    public string UnlockedAtUtc { get; set; }

    // Domain -> DTO
    public static PlayerAchievementDTO FromDomain(PlayerAchievement pa)
    {
        return new PlayerAchievementDTO
        {
            AchievementId = pa.AchievementId,
            Current = pa.Progress.Current,
            IsUnlocked = pa.IsUnlocked,
            UnlockedAtUtc = pa.UnlockedAtUtc.HasValue
                ? pa.UnlockedAtUtc.Value.ToUniversalTime().ToString("o")
                : null
        };
    }

    // DTO -> Domain
    public PlayerAchievement ToDomain()
    {
        var pa = new PlayerAchievement(AchievementId);
        pa.Progress.SetCurrent(Current);
        if (IsUnlocked)
        {
            pa.Unlock(FromUtc(UnlockedAtUtc) ?? DateTime.UtcNow);
        }

        return pa;
    }

    private DateTime? FromUtc(string s)
        => string.IsNullOrEmpty(s)
            ? (DateTime?)null
            : DateTime.Parse(s, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
}