using Fusion;
using UnityEngine;

public class PlayerAchievementNetAgent : NetworkBehaviour, IAchievementServerPort
{
    public override void Spawned()
    {
        if (Object.HasStateAuthority) { // 서버/Host만 바인딩
            AchievementManager.Instance.ServerPort = this;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Object.HasStateAuthority && AchievementManager.Instance.ServerPort == this){
            AchievementManager.Instance.ServerPort = null;
        }
    }

    /// 로컬에서 업적 이벤트 처리(즉시 평가/토스트/스냅샷)
    public void HandleEventServer(PlayerRef player, AchievementEvent e)
    {
        RPC_HandleEvent(player, e);
    }
    
    /// 편의성 메소드 수치 추가 후 재평가
    public void AddMetricAndReevaluateServer(PlayerRef player, string key, long delta, bool emitToasts = true)
    {
        RPC_AddMetricAndReevaluate(player, key, delta, emitToasts);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HandleEvent([RpcTarget] PlayerRef player, AchievementEvent e)
    {
        AchievementManager.Instance.HandleEventLocal(e);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_AddMetricAndReevaluate([RpcTarget] PlayerRef player, string key, long delta, bool emitToasts)
    {
        AchievementManager.Instance.AddMetricAndReevaluateLocal(key, delta, emitToasts);
    }
}