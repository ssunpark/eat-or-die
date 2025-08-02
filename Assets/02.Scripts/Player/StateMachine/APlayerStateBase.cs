using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public abstract class APlayerStateBase : State<APlayerStateBase>
{
    protected PlayerController _controller;
    protected StatManager _stat;
    protected ResourceManager _resource;
    protected bool _shouldAbortStateEarly = false; 
    protected NetworkInputData _input;
    private int _lastInputCacheTick = -1;

    public APlayerStateBase(PlayerController controller)
    {
        _controller = controller;
        _stat = controller.Stat;
        _resource = controller.Resource;
    }

    protected void EvaluateTargetOrAbort(System.Func<bool> canExecuteAction)
    {
        if (_controller.HasInputAuthority)
        {
            // 입력 권한이 있는 쪽이 조건 판단
            _shouldAbortStateEarly = !canExecuteAction();
        }
    }
    protected bool TryCacheInput()
    {
        if (Machine == null) return false;

        if (Machine.Runner.Tick == _lastInputCacheTick)
            return true; // 이미 캐시된 상태

        if(Machine.Runner.TryGetInputForPlayer<NetworkInputData>(_controller.Object.InputAuthority, out _input))
        {
            _lastInputCacheTick = Machine.Runner.Tick;
            return true; // 입력 캐시 성공
        }
        return false;
    }

    protected bool CanInteract()
    {
        if (!_controller.HasStateAuthority) return false;
        if (!TryCacheInput()) return false;
        if (!_input.WasPressed(EButtons.Interact)) return false;
        return _controller.CanInteract;
    }

    protected bool CanUseItem()
    {
        if (!_controller.HasStateAuthority) return false;
        if (!TryCacheInput()) return false;
        if (!_input.WasPressed(EButtons.UseItem)) return false;
        return _controller.CanUseItem;
    }

    protected bool CanStartAttack()
    {
        if (!_controller.HasStateAuthority) return false;
        if (!TryCacheInput()) return false;
        if (!_input.WasPressed(EButtons.Attack)) return false;

        float cooldown = Mathf.Max(1f / _stat.GetStat(EStatType.AttackSpeed), 0.01f);
        return _controller.LastAttackTime + cooldown < Machine.Runner.LocalRenderTime;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    protected void RPC_RequestExit()
    {
        Debug.Log($"RPC_RequestExit called for state {StateId} on player {_controller.Object.InputAuthority}");
        Machine.ForceDeactivateState(StateId);
    }
}