using System;
using System.Collections.Generic;

public class AchievementPresenter : IAchievementPresenter
{
    public event Action<IReadOnlyList<AchievementViewModel>> OnSnapshot;
    public event Action<AchievementViewModel> OnToast;
    
    public void PublishSnapshot(IReadOnlyList<AchievementViewModel> dto)
    {
        OnSnapshot?.Invoke(dto);
    }

    public void PublishUnlockedToast(AchievementViewModel viewModel)
    {
        OnToast?.Invoke(viewModel);
    }
}