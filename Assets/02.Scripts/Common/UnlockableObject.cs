using Fusion;
using UnityEngine;

public class UnlockableObject : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnUnlock))]
    private bool IsUnlocked { get; set; }

    [SerializeField]
    private GameObject _unlockTarget;

    public override void Spawned()
    {
        OnUnlock();
    }

    private void OnUnlock()
    {
        gameObject.layer = LayerMask.NameToLayer(IsUnlocked ? "Default" : "Interactable");
        _unlockTarget.SetActive(IsUnlocked);
    }

    public void Unlock()
    {
        Debug.Log("Unlock");
        RPC_Unlock();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Unlock()
    {
        if (!HasStateAuthority)
        {
            return;
        }
         
        Debug.Log("Unlocking " + _unlockTarget.name);
        IsUnlocked = true;
    }
}