using System;
using System.Collections.Generic;

public interface IAchievementPresenter {
    public event Action<IReadOnlyList<AchievementViewModel>> OnSnapshot;
    public event Action<AchievementViewModel> OnToast;
    public void PublishSnapshot(IReadOnlyList<AchievementViewModel> dto);
    public void PublishUnlockedToast(AchievementViewModel viewModel);
}