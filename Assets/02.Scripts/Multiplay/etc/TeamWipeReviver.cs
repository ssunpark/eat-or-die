using System.Linq;
using Fusion;
using UnityEngine;

/// <summary>
/// 전원 사망 시 즉시 부활 처리
/// </summary>
public class TeamWipeReviver : NetworkBehaviour
{
    [SerializeField] private bool _useInstantRevive = true;
    [SerializeField] private bool _log = false;

    private bool _wasAllDead;

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        if (PlayerInfoManager.Instance == null) return;

        var players = PlayerInfoManager.PlayerControllers.Values;
        if (players == null || players.Count == 0) return;

        // PlayerFSM의 IsDead 상태를 검사
        bool allDead = players.All(p => p != null && p.PlayerFSM != null && p.PlayerFSM.IsDead);

        if (allDead && !_wasAllDead)
        {
            if (_log) Debug.Log("[TeamWipeReviver] All players dead. Reviving all...");

            foreach (var player in players)
            {
                if (player == null) continue;

                if (_useInstantRevive)
                    player.InstantRevive();
                else
                    player.Revive();
            }

            _wasAllDead = true;
        }
        else if (!allDead)
        {
            _wasAllDead = false;
        }
    }
}
