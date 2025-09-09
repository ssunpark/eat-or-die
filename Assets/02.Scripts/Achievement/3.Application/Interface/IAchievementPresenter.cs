using System;
using System.Collections.Generic;

public interface IAchievementPresenter {
    public event Action<IReadOnlyList<AchievementDto>> OnSnapshot;
    public event Action<AchievementDto> OnToast;
    public void PublishSnapshot(IReadOnlyList<AchievementDto> dto);
    public void PublishUnlockedToast(AchievementDto dto);
}