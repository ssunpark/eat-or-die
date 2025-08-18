using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class StageManager : NetworkBehaviour
{
    public List<PlayerRef> PlayerList;
    public int StageIndex;
    
    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        
        PlayerList = new List<PlayerRef>();
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_StageEnter()
    {
        Debug.Log($"Stage {StageIndex} Enter");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_StageExit()
    {
        Debug.Log($"Stage {StageIndex} Exit");
    }
}
