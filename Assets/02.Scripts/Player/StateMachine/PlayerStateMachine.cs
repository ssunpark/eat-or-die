using System.Collections.Generic;
using Fusion;
using UnityEngine;

// 현재 플레이어 상태 전환 관리
public class PlayerStateMachine : NetworkBehaviour, IDamageable
{
    //[Networked, OnChangedRender(nameof(OnStateChanged))] 
    public EPlayerState CurrentState { get; set; }

    private Dictionary<EPlayerState, APlayerState> _states;
    private APlayerState _activeState;

    private PlayerController _controller;


    //이동중일 때 배고픔 감소 속도 조절을 위한 타이머
    [HideInInspector]
    public float MoveSatietyTimer;
    // 몇초 이동했을 때 배고픔 감소가 일어날지
    public float MoveStatietyInterval = 3f;

    public override void Spawned()
    {
        _controller = GetComponent<PlayerController>();
        _states = new Dictionary<EPlayerState, APlayerState>
        {
            { EPlayerState.Idle, new PlayerIdleState(this, _controller) },
            { EPlayerState.Move, new PlayerMoveState(this, _controller) },
            { EPlayerState.Attack, new PlayerAttackState(this, _controller) },
            { EPlayerState.Hit, new PlayerHitState(this, _controller) },
            /*
            { EPlayerState.Interact, new PlayerInteractState(this, _controller) },
            { EPlayerState.Cooking, new PlayerCookingState(this, _controller) },
            { EPlayerState.Down, new PlayerDownState(this, _controller) },
            { EPlayerState.Dead, new PlayerDeadState(this, _controller) },
             */
        };

        CurrentState = EPlayerState.Idle;
        _activeState = _states[CurrentState];
        _activeState.Enter();

        _controller.Resource.OnSatietyChanged += Resource_OnSatietyChanged;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _controller.Resource.OnSatietyChanged -= Resource_OnSatietyChanged;
        _activeState?.Exit();
        _activeState = null;
        _states.Clear();
        _states = null;
    }

    private void Resource_OnSatietyChanged(float arg1, float arg2)
    {
        // 배고픔이 감소할 때마다 상태를 업데이트
        // 배고픔이 30% 이하가 되면 플레이어 상태를 광폭화 상태로 변경
        if (arg1 <= _controller.Stat.GetStat(EStatType.MaxSatiety) * 0.3f)
        {
            //ChangeState(EPlayerState.rhkdvhrghk);
            //return;
        }
        // 배고픔이 0 이하가 되면 플레이어 상태를 죽음으로 변경
        if (arg1 <= 0)
        {
            // ChangeState(EPlayerState.Dead);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(HasInputAuthority) _activeState?.Tick();
    }

    //private void OnStateChanged()
    //{
    //    _activeState?.Exit();
    //    _activeState = _states[CurrentState];
    //    _activeState.Enter();
    //}

    public void ChangeState(EPlayerState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
        _activeState?.Exit();
        _activeState = _states[CurrentState];
        _activeState.Enter();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_OrderChangeState(EPlayerState newState)
    {
        ChangeState(newState);
    }

    public void TakeDamage(int amount, PlayerRef attacker)
    {
        if (!HasStateAuthority) return;

        var dmg = amount * (100 / (100 + _controller.Stat.GetStat(EStatType.Armor)));

        _controller.Resource.ConsumeSatiety(dmg);
        RPC_OrderChangeState(EPlayerState.Hit);
    }

    public bool CanMove
    {
        get
        {
            if (_states.TryGetValue(CurrentState, out var state))
            {
                return state.CanMove;
            }
            return false;
        }
    }
}