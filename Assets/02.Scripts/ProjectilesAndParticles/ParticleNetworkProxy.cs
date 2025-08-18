using Fusion;
using UnityEngine;

public class ParticleNetworkProxy : NetworkBehaviour
{
    public static ParticleNetworkProxy Instance { get; private set; }

    public override void Spawned()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_PlayParticle(string key, Vector3 position, Quaternion rotation)
    {
        ParticleManager.Instance?.PlayByKeyLocal(key, position, rotation);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestPlayParticle(string key, Vector3 position, Quaternion rotation)
    {
        Rpc_PlayParticle(key, position, rotation);
    }
}
