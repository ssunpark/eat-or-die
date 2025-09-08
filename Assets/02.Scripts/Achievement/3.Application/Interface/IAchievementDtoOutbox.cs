using System.Collections.Generic;

public interface IAchievementDtoOutbox {
    public void PublishDto(IReadOnlyList<AchievementDto> dto);
    public void PublishUnlockedToast(int achievementId);
}