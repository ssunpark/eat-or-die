using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;

public class FirestorePlayerAchievementRepository : IPlayerAchievementRepository
{
    private readonly FirebaseFirestore _db;
    private readonly string _userId;
    private readonly string _characterId;

    // 로컬 캐시(선택): Get/Upsert호출 빈번 시 메모리 캐시 유지
    private readonly Dictionary<int, PlayerAchievement> _cache = new();

    public FirestorePlayerAchievementRepository(FirebaseFirestore db, string userId, string characterId)
    {
        _db = db;
        _userId = userId;
        _characterId = characterId;
    }

    private CollectionReference AchievementsCol =>
        _db.Collection("Users").Document(_userId)
            .Collection("Characters").Document(_characterId)
            .Collection("Achievements");

    public PlayerAchievement Get(int achievementId)
    {
        if (!_cache.TryGetValue(achievementId, out var pa))
            _cache[achievementId] = pa = new PlayerAchievement(achievementId);
        return pa;
    }

    public async void Upsert(PlayerAchievement pa)
    {
        _cache[pa.AchievementId] = pa;
        await SaveAsync(pa.AchievementId);
    }

    public IReadOnlyList<PlayerAchievement> GetAll() => new List<PlayerAchievement>(_cache.Values);

    // ==== Firestore 연동 메소드 ====

    // 단일 저장
    public async UniTask SaveAsync(int achievementId)
    {
        if (!_cache.TryGetValue(achievementId, out var pa))
            return;

        var dto = PlayerAchievementDTO.FromDomain(pa);

        await AchievementsCol.Document(achievementId.ToString()).SetAsync(dto);
    }

    // 모두 저장
    public async UniTask SaveAsync()
    {
        var batch = _db.StartBatch();
        foreach (var pa in _cache.Values)
        {
            var dto = PlayerAchievementDTO.FromDomain(pa);
            var doc = AchievementsCol.Document(pa.AchievementId.ToString());
            batch.Set(doc, dto);
        }

        await batch.CommitAsync();
    }

    // 로드
    public async UniTask LoadAsync()
    {
        var snap = await AchievementsCol.GetSnapshotAsync();

        _cache.Clear();
        foreach (var doc in snap.Documents)
        {
            var dto = doc.ConvertTo<PlayerAchievementDTO>();
            var pa = dto.ToDomain();
            _cache[pa.AchievementId] = pa;
        }
    }
}