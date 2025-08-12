using Fusion; // Add Fusion for NetworkInputData
using Fusion.Addons.FSM; // Add Fusion FSM for PlayerStateMachine
using UnityEngine;
public class PlayerUseItemState : APlayerStateBase, IAnimationActionNotify
{
    public PlayerUseItemState(PlayerFSM controller) : base(controller)
    {
        AnimState = "UseItem";
        StateId = (int)EPlayerState.UseItem;
        _shouldPlayAnimation = false;
    }

    private NetworkObject _target;
    protected override void OnEnterState()
    {
        base.OnEnterState();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        if(_fsm.ItemUseTarget == null)
        {
            Machine.ForceActivateState<PlayerIdleState>();
        }

    }

    protected override void OnEnterStateRender()
    {
        base.OnEnterStateRender();
        _fsm.CanInteract = false;
        _fsm.CanUseItem = false;
        PlayUseVfx(EUsePhase.Start, _fsm.transform.position+Vector3.up);
        if (_fsm.HasStateAuthority || _fsm.HasInputAuthority)
        {
            _target = _fsm.ItemUseTarget;
        }
        string desired = _fsm.UseItemMode == EUseItemMode.Give ? "UseItem_Give" : "UseItem";
        AnimState = desired;
        Anim.CrossFadeInFixedTime(AnimState, AnimTransitionLength);
    }

    void IAnimationActionNotify.OnActionMoment()
    {
        if (!_fsm.HasInputAuthority) return;
        if (_target == null)
        {
            Debug.LogWarning("PlayerUseItemState: Target is null. Cannot use item.");
            return;
        }

        if(_fsm.UseItemMode == EUseItemMode.Self)
        {
            PlayUseVfx(EUsePhase.Success, _fsm.transform.position + (Vector3.up * 0.5f));
        }
        else
        {
            PlayUseVfx(EUsePhase.Success, _target.transform.position + (Vector3.up * 0.5f));
        }
        _fsm.ItemHolder.UseItem(_target.gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    protected void RPC_UseItemOrder(NetworkObject target)
    {
        if (target == null)
        {
            Debug.LogWarning("PlayerUseItemState: Target is null. Cannot use item.");
            return;
        }
        _fsm.ItemHolder.UseItem(target.gameObject);
    }
    protected override void OnFixedUpdateInput()
    {
        KCC.Move(Vector3.zero);
        if (Machine.StateTime >= _fsm.PlayerNetworkObject.AnimationClipLengths[AnimState])
        {
            Debug.Log(_fsm.PlayerNetworkObject.AnimationClipLengths[AnimState]);
            RequestActivateState();
        }
    }

    private void PlayUseVfx(EUsePhase phase, Vector3 worldPos)
    {
        var go = _fsm.ItemHolder?.HeldItemObject;

        if (go != null && go.TryGetComponent<IUseVfxProvider>(out var vfx))
        {
            var key = vfx.GetEffectKey(phase);
            if (string.IsNullOrEmpty(key)) return;

            if (vfx.MustBeChild)
            {
                var parent = vfx.GetUseSpawnPoint() ?? go.transform;
                ParticleManager.Instance.PlayByKeyLocalAsChild(
                    key, parent, Vector3.zero, Quaternion.identity
                );
            }
            else
            {
                var rot = Quaternion.identity;
                ParticleManager.Instance.RPC_RequestPlayParticle(key, worldPos, rot);
            }
        }
    }
}