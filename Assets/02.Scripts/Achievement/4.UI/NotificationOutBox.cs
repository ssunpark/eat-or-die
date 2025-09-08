using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationOutBox : MonoBehaviour,IAchievementDtoOutbox
{
    public void PublishDto(IReadOnlyList<AchievementDto> dto)
    {
        
    }

    public void PublishUnlockedToast(int achievementId)
    {
        var snapshot = AchievementManager.Instance.GetAchievementDTO();
        var dto = snapshot.FirstOrDefault(a => a.Id == achievementId);
        
        if (dto != null)
        {
            Debug.LogWarning("업적 달성");
            // UI 토스트
        }
        else
        {
            Debug.LogWarning("없는 업적입니다.");
        }
    }
}