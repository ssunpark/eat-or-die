using System.Collections.Generic;

public interface IPlayerAchievementRepository {
    public PlayerAchievement Get(int achievementId);
    public void Upsert(PlayerAchievement pa);
    public IReadOnlyList<PlayerAchievement> GetAll();
}