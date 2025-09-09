using System;
using System.Linq;

public class ProcessAchievementEventUseCase
{
    private readonly IAchievementCatalog _catalog;
    private readonly IPlayerAchievementRepository _repo;
    private readonly IAchievementProgressQuery _progress;
    private readonly AchievementEvaluator _evaluator;
    private readonly IAchievementPresenter? _outbox;

    public ProcessAchievementEventUseCase(
        IAchievementCatalog catalog,
        IPlayerAchievementRepository repo,
        IAchievementProgressQuery progress,
        Func<DateTime> utcNowProvider,
        IAchievementPresenter? outbox = null)
    {
        _catalog = catalog;
        _repo = repo;
        _progress = progress;
        _outbox = outbox;

        _evaluator = new AchievementEvaluator(
            (key) => _progress.GetValue(key),
            utcNowProvider
        );

        _evaluator.OnUnlocked += OnUnlocked;
    }

    private void OnUnlocked(AchievementUnlocked unlock)
    {
        if (_outbox == null) return;
        if (!_catalog.TryGet(unlock.AchievementId, out var ach)) return;
        var pa  = _repo.Get(unlock.AchievementId);
        var dto = AchievementDto.From(ach, pa);
        _outbox.PublishUnlockedToast(dto);
    }

    public void Handle(AchievementEvent e) {
        var catalog = _catalog.GetAll();

        _evaluator.Evaluate(
            e,
            catalog,
            achId => _repo.Get(achId),
            pa => _repo.Upsert(pa)
        );

        if (_outbox != null) {
            var list = catalog
            .Select(ach => AchievementDto.From(ach, _repo.Get(ach.Id)))
            .ToArray();
            _outbox.PublishSnapshot(list);
        }
    }
}