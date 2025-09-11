using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class AchievementManager : BehaviourSingleton<AchievementManager>
{
    private const string ACHIEVEMENT_CSV_PATH = "/AchievementCSV/Achievements.csv";

    // 의존성
    private IAchievementCatalog _catalog;
    private IPlayerAchievementRepository _repo;
    private IAchievementProgressQuery _progress;
    private IAchievementPresenter _presenter;
    private IAchievementServerPort _serverPort;
    
    public IAchievementPresenter Presenter => _presenter;
    public IAchievementServerPort ServerPort { get => _serverPort; set => _serverPort = value; }

    // UseCases
    private ProcessAchievementEventUseCase _processUC;
    private ReevaluateAllAchievementsUseCase _reevalUC;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // CSV → 카탈로그
        var list = LoadAchievementsFromCsv();
        _catalog = new InMemoryAchievementCatalog(list);
        _progress = new LocalAchievementProgressQuery();
        _presenter = new AchievementPresenter();

        Debug.Log($"[AchievementManager] Catalog loaded: {_catalog.GetAll().Count} achievements");
    }

    /// 로컬에서 업적 이벤트 처리(즉시 평가/토스트/스냅샷)
    public void HandleEventLocal(AchievementEvent e)
    {
        _processUC.Handle(e);
    }

    /// 서버에서 업적 이벤트 처리(즉시 평가/토스트/스냅샷)
    public void HandleEventServer(PlayerRef player, AchievementEvent e)
    {
        ServerPort?.HandleEventServer(player, e);
    }

    /// 로컬에서 메트릭 변경 후 전체 재평가
    public void ReevaluateAllLocal()
    {
        _reevalUC.Handle();
    }

    /// 모두 재평가 후 토스트
    public void ReevaluateAllLocalWithToasts()
    {
        _reevalUC.Handle(emitToasts: true);
    }

    /// 편의성 메소드 수치 추가 후 재평가
    public void AddMetricAndReevaluateLocal(string key, long delta, bool emitToasts = true)
    {
        var current = _progress.GetValue(key);
        SetMetricLocal(key, current + delta);
        if (emitToasts)
        {
            ReevaluateAllLocalWithToasts();
        }
        else
        {
            ReevaluateAllLocal();
        }
    }

    /// 서버에서 업적 이벤트 처리(즉시 평가/토스트/스냅샷)
    public void AddMetricAndReevaluateServer(PlayerRef player, string key, long delta, bool emitToasts = true)
    {
        ServerPort?.AddMetricAndReevaluateServer(player, key, delta, emitToasts);
    }

    /// 로컬 메트릭 세팅(예: kills.total, currency.gold 등)
    public void SetMetricLocal(string key, long value)
    {
        if (_progress is LocalAchievementProgressQuery query)
            query.Set(key, value);
    }

    /// DTO 조회
    public IReadOnlyList<AchievementViewModel> GetAchievementDTOList()
    {
        return _catalog.GetAll()
            .Select(ach => AchievementViewModel.From(ach, _repo.Get(ach.Id)))
            .ToArray();
    }

    public async UniTask SetRepository(IPlayerAchievementRepository repository)
    {
        _repo = repository;
        
        _processUC = new ProcessAchievementEventUseCase(
            _catalog, _repo, _progress, () => DateTime.Now, _presenter);

        _reevalUC = new ReevaluateAllAchievementsUseCase(
            _catalog, _repo, _progress, () => DateTime.Now, _presenter);
        
        // 로드
        await _repo.LoadAsync();
        InitAchievementProgress();
        ReevaluateAllLocal();
    }
    
    private void InitAchievementProgress()
    {
        foreach (var ach in _catalog.GetAll())
        {
            var pa = _repo.Get(ach.Id);

            var cr = ach.Criteria;
            
            // 목표치 초기화
            pa.Progress.SetTarget(cr.Target);

            // 진행도 초기화
            // 현재 메트릭이 비어 있으면(=0) 최소한 저장된 진행도/타깃 중 큰 값으로 채움
            var curMetric = _progress.GetValue(cr.StatKey);
            var curSaved  = (long)pa.Progress.Current;
            var minNeeded = pa.IsUnlocked ? Math.Max(curSaved, cr.Target) : curSaved;

            if (curMetric < minNeeded)
                SetMetricLocal(cr.StatKey, minNeeded);
        }
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