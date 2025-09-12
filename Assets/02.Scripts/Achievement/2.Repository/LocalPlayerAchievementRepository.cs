using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

/// 로컬 전용 PlayerAchievement 저장소
public class LocalPlayerAchievementRepository : IPlayerAchievementRepository
{
    private readonly Dictionary<int, PlayerAchievement> _achievementDict = new();

    public PlayerAchievement Get(int achievementId)
    {
        if (!_achievementDict.TryGetValue(achievementId, out var pa))
            _achievementDict[achievementId] = pa = new PlayerAchievement(achievementId);
        return pa;
    }

    public void Upsert(PlayerAchievement pa)
    {
        _achievementDict[pa.AchievementId] = pa;
    }

    public IReadOnlyList<PlayerAchievement> GetAll()
    {
        return _achievementDict.Values.ToList();
    }
    
    public UniTask LoadAsync() => UniTask.CompletedTask;
    public UniTask SaveAsync() => UniTask.CompletedTask;
}