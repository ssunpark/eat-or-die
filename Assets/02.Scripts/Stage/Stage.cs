using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Stage : NetworkBehaviour
{
    public List<PlayerRef> PlayerList;
    public int StageIndex;
    public string StageName;
    
    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        
        PlayerList = new List<PlayerRef>();
    }

    public void AlertEnterStage()
    {
        Debug.Log($"{StageName} entered");
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StageEnter(PlayerRef player)
    {
        Debug.Log($"Stage {StageIndex} Enter");
        PlayerList.Add(player);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StageExit(PlayerRef player)
    {
        Debug.Log($"Stage {StageIndex} Exit");
        PlayerList.Remove(player);
    }
}