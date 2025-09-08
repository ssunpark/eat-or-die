using System;
using System.Collections.Generic;

/// 업적 평가기
  public class AchievementEvaluator {
    private readonly Func<string, long> _getStat; // (playerId, statKey) -> value
    private readonly Func<DateTime> _utcNow;

    // 도메인 이벤트 발행 훅(애플리케이션에서 구독)
    public event Action<AchievementUnlocked> OnUnlocked;

    public AchievementEvaluator(Func<string, long> getStat, Func<DateTime> utcNow) {
      _getStat = getStat; _utcNow = utcNow;
    }

    /// 단일 이벤트를 반영하여 해당 플레이어의 업적들을 평가
    public void Evaluate(AchievementEvent e,
                         IReadOnlyList<Achievement> catalog,
                         Func<int, PlayerAchievement> getPlayerAch,
                         Action<PlayerAchievement> savePlayerAch) {

      foreach (var ach in catalog) {
        var pa = getPlayerAch(ach.Id);
        if (pa.IsUnlocked) continue;

        // 진행도 증분
        ach.Criteria.Accumulate(pa.Progress, e, statKey => _getStat(statKey));

        // 달성 판정
        if (ach.Criteria.IsSatisfied(pa.Progress)) {
          pa.Unlock(_utcNow());
          savePlayerAch(pa);
          OnUnlocked?.Invoke(new AchievementUnlocked(ach.Id, pa.UnlockedAtUtc!.Value));
        } else {
          savePlayerAch(pa);
        }
      }
    }

    /// 통계 값이 바뀌었을 때(예: OnStatChanged) 전체 재평가
    public void ReEvaluateAll(int playerId,
                              IReadOnlyList<Achievement> catalog,
                              Func<int, PlayerAchievement> getPlayerAch,
                              Action<PlayerAchievement> savePlayerAch) {
      var dummyEvent = new AchievementEvent("__STAT_REFRESH__", 0);
      Evaluate(dummyEvent, catalog, getPlayerAch, savePlayerAch);
    }
  }