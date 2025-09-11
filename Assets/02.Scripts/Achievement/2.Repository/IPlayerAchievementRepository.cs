using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public interface IPlayerAchievementRepository {
    public PlayerAchievement Get(int achievementId);
    public void Upsert(PlayerAchievement pa);
    public IReadOnlyList<PlayerAchievement> GetAll();
    public UniTask LoadAsync();
    public UniTask SaveAsync();
}