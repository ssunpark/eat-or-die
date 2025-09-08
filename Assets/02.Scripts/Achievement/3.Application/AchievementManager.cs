using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : BehaviourSingleton<AchievementManager>
{
    private const string ACHIEVEMENT_CSV_PATH = "/AchievementCSV/Achievements.csv";

    // 의존성
    private IAchievementCatalog _catalog;
    private IPlayerAchievementRepository _repo;
    private IAchievementProgressQuery _progress;
    private IAchievementDtoOutbox _outbox;

    // UseCases
    private ProcessAchievementEventUseCase _processUC;
    private ReevaluateAllAchievementsUseCase _reevalUC;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // CSV → 카탈로그
        var list = LoadAchievementsFromCsv();
        _catalog = new InMemoryAchievementCatalog(list);
        _repo = new LocalPlayerAchievementRepository();
        _progress = new LocalAchievementProgressQuery();

        // 로컬 UI 전용 Outbox (토스트/리스트)
        _outbox = new NotificationOutBox();

        // UseCases
        _processUC = new ProcessAchievementEventUseCase(
            _catalog, _repo, _progress, () => DateTime.UtcNow, _outbox);

        _reevalUC = new ReevaluateAllAchievementsUseCase(
            _catalog, _repo, _progress, () => DateTime.UtcNow, _outbox);

        Debug.Log($"[AchievementManager] Catalog loaded: {_catalog.GetAll().Count} achievements");
    }

    /// 로컬에서 업적 이벤트 처리(즉시 평가/토스트/스냅샷)
    public void HandleEventLocal(AchievementEvent e)
    {
        _processUC.Handle(e);
    }

    /// 로컬에서 메트릭 변경 후 전체 재평가
    public void ReevaluateAllLocal(int playerId)
    {
        _reevalUC.Handle(playerId);
    }

    /// 로컬 메트릭 세팅(예: kills.total, currency.wallet 등)
    public void SetMetricLocal(string key, long value)
    {
        if (_progress is LocalAchievementProgressQuery query)
            query.Set(key, value);
    }

    /// DTO 조회
    public IReadOnlyList<AchievementDto> GetAchievementDTO()
    {
        return _catalog.GetAll()
            .Select(ach => AchievementDto.From(ach, _repo.Get(ach.Id)))
            .ToArray();
    }

    private IReadOnlyList<Achievement> LoadAchievementsFromCsv()
    {
        string path = $"{Application.streamingAssetsPath}{ACHIEVEMENT_CSV_PATH}";
        try
        {
            var raws = CSVLoader<AchievementRawData>.LoadCSV(path);
            return AchievementFactory.Create(raws);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AchievementManager] CSV load failed: {ex.Message}\n{path}");
            return Array.Empty<Achievement>();
        }
    }
}