using System;
using System.Linq;

public class ReevaluateAllAchievementsUseCase {
    private readonly IAchievementCatalog _catalog;
    private readonly IPlayerAchievementRepository _repo;
    private readonly IAchievementProgressQuery _progress;
    private readonly AchievementEvaluator _evaluator;
    private readonly IAchievementDtoOutbox? _outbox;

    public ReevaluateAllAchievementsUseCase(
        IAchievementCatalog catalog,
        IPlayerAchievementRepository repo,
        IAchievementProgressQuery progress,
        Func<DateTime> utcNowProvider,
        IAchievementDtoOutbox? outbox = null)
    {
        _catalog = catalog; _repo = repo; _progress = progress; _outbox = outbox;
        _evaluator = new AchievementEvaluator((key) => _progress.GetValue(key), utcNowProvider);
    }

    public void Handle(int playerId) {
        _evaluator.ReEvaluateAll(
            playerId,
            _catalog.GetAll(),
            achId => _repo.Get(achId),
            pa => _repo.Upsert(pa)
        );

        if (_outbox != null) {
            var list = _catalog.GetAll()
                .Select(ach => AchievementDto.From(ach, _repo.Get(ach.Id)))
                .ToArray();
            _outbox.PublishDto(list);
        }
    }
}