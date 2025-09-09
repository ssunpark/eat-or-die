using System;
using System.Collections.Generic;

public class AchievementPresenter : IAchievementPresenter
{
    public event Action<IReadOnlyList<AchievementDto>> OnSnapshot;
    public event Action<AchievementDto> OnToast;
    
    public void PublishSnapshot(IReadOnlyList<AchievementDto> dto)
    {
        OnSnapshot?.Invoke(dto);
    }

    public void PublishUnlockedToast(AchievementDto dto)
    {
        OnToast?.Invoke(dto);
    }
}