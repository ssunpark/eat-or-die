using System.Collections.Generic;
using Fusion;
using UnityEngine;

// 현재 플레이어 상태 전환 관리
public class PlayerStateMachine : NetworkBehaviour
{
    [Networked] public EPlayerState CurrentState { get; set; }

    private EPlayerState _cachedState;

    private Dictionary<EPlayerState, PlayerStateBase> _states;
    private PlayerStateBase _activeState;

    private PlayerController _controller;

    //이동중일 때 배고픔 감소 속도 조절을 위한 타이머
    [HideInInspector]
    public float MoveSatietyTimer;
    // 몇초 이동했을 때 배고픔 감소가 일어날지
    public float MoveStatietyInterval = 3f;

    public override void Spawned()
    {
        _controller = GetComponent<PlayerController>();
        _states = new Dictionary<EPlayerState, PlayerStateBase>
        {
            { EPlayerState.Idle, new PlayerIdleState(this, _controller) },
            { EPlayerState.Move, new PlayerMoveState(this, _controller) },
            /*
            { EPlayerState.Attack, new PlayerAttackState(this, _controller) },
            { EPlayerState.Interact, new PlayerInteractState(this, _controller) },
            { EPlayerState.Cooking, new PlayerCookingState(this, _controller) },
            { EPlayerState.Down, new PlayerDownState(this, _controller) },
            { EPlayerState.Dead, new PlayerDeadState(this, _controller) },
             */
        };

        if (Object.HasStateAuthority)
        {
            CurrentState = EPlayerState.Idle;
        }
        _cachedState = CurrentState;
        _activeState = _states[CurrentState];
        _activeState.Enter();
    }

    public override void FixedUpdateNetwork()
    {
        if (_cachedState != CurrentState)
        {
            OnStateChanged(_cachedState, CurrentState);
            _cachedState = CurrentState;
        }

        if (Object.HasInputAuthority)
        {
            _activeState?.Tick();
        }
    }

    private void OnStateChanged(EPlayerState oldState, EPlayerState newState)
    {
        _activeState?.Exit();
        _activeState = _states[newState];
        _activeState.Enter();
    }

    public void ChangeState(EPlayerState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
    }
}