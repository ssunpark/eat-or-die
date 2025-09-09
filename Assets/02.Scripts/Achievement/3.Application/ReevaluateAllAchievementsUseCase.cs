using System;
using System.Linq;

public class ReevaluateAllAchievementsUseCase {
    private readonly IAchievementCatalog _catalog;
    private readonly IPlayerAchievementRepository _repo;
    private readonly IAchievementProgressQuery _progress;
    private readonly AchievementEvaluator _evaluator;
    private readonly IAchievementPresenter? _outbox;
    
    private bool _emitToasts;

    public ReevaluateAllAchievementsUseCase(
        IAchievementCatalog catalog,
        IPlayerAchievementRepository repo,
        IAchievementProgressQuery progress,
        Func<DateTime> utcNowProvider,
        IAchievementPresenter? outbox = null)
    {
        _catalog = catalog; _repo = repo; _progress = progress; _outbox = outbox;
        _evaluator = new AchievementEvaluator((key) => _progress.GetValue(key), utcNowProvider);
        
        _evaluator.OnUnlocked += OnUnlocked;
    }
    
    private void OnUnlocked(AchievementUnlocked unlock) {
        if (!_emitToasts || _outbox == null) return;
        if (!_catalog.TryGet(unlock.AchievementId, out var ach)) return;
        var pa  = _repo.Get(unlock.AchievementId);
        var dto = AchievementDto.From(ach, pa);
        _outbox.PublishUnlockedToast(dto);
    }

    public void Handle(bool emitToasts = false) {
        _emitToasts = emitToasts;

        _evaluator.ReEvaluateAll(
            _catalog.GetAll(),
            achId => _repo.Get(achId),
            pa => _repo.Upsert(pa)
        );

        // 스냅샷은 항상 발행(리스트 UI 업데이트용)
        if (_outbox != null) {
            var list = _catalog.GetAll()
                .Select(ach => AchievementDto.From(ach, _repo.Get(ach.Id)))
                .ToArray();
            _outbox.PublishSnapshot(list);
        }

        _emitToasts = false; // 기본값 재설정
    }
}