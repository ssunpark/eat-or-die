using System.Collections.Generic;
using Fusion;
using UnityEngine;

// 현재 플레이어 상태 전환 관리
public class PlayerStateMachine : NetworkBehaviour, IDamageable
{
    public EPlayerState CurrentStateForDebug; // 디버그용 현재 상태 (인스펙터에서 확인 가능)
    public EPlayerState CurrentState { get; set; }
    private Dictionary<EPlayerState, APlayerState> _states;
    private APlayerState _activeState;

    #region References
    [HideInInspector] public PlayerInteractions Interact;
    private PlayerController _controller;
    #endregion

    public IInteractable Interactable;
    public IUsable Usable;

    //이동중일 때 배고픔 감소 속도 조절을 위한 타이머
    [HideInInspector]
    public float MoveSatietyTimer;
    // 몇초 이동했을 때 배고픔 감소가 일어날지
    public float MoveStatietyInterval = 1f;


    public override void Spawned()
    {
        _controller = GetComponent<PlayerController>();
        Interact = GetComponent<PlayerInteractions>();
        _states = new Dictionary<EPlayerState, APlayerState>
        {
            { EPlayerState.Idle, new PlayerIdleState(this, _controller) },
            { EPlayerState.Move, new PlayerMoveState(this, _controller) },
            { EPlayerState.Attack, new PlayerAttackState(this, _controller) },
            { EPlayerState.Hit, new PlayerHitState(this, _controller) },
            { EPlayerState.UsingTool, new PlayerUsingItemState(this, _controller) },
            { EPlayerState.Cooking, new PlayerCookingState(this, _controller) },
            { EPlayerState.Interact, new PlayerInteractState(this, _controller) },
            //{ EPlayerState.Dead, new PlayerDeadState(this, _controller) },
            /*{ EPlayerState.Down, new PlayerDownState(this, _controller) },
            
             */
        };

        CurrentState = EPlayerState.Idle;
        _activeState = _states[CurrentState];
        _activeState.Enter();

        _controller.Resource.OnHungerChanged += Resource_OnSatietyChanged;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        _controller.Resource.OnHungerChanged -= Resource_OnSatietyChanged;
        _activeState?.Exit();
        _activeState = null;
        _states.Clear();
        _states = null;
    }

    private void Resource_OnSatietyChanged(float arg1, float arg2)
    {
        // 배고픔이 감소할 때마다 상태를 업데이트
        // 배고픔이 30% 이하가 되면 플레이어 상태를 광폭화 상태로 변경
        if (arg1 <= _controller.Stat.GetStat(EStatType.MaxHunger) * 0.3f)
        {
            //ChangeState(EPlayerState.rhkdvhrghk);
            //return;
        }
        // 배고픔이 0 이하가 되면 플레이어 상태를 죽음으로 변경
        if (arg1 <= 0)
        {
            //ChangeState(EPlayerState.Dead);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if(HasInputAuthority) _activeState?.Tick();
    }

    public void ChangeState(EPlayerState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
        _activeState?.Exit();
        _activeState = _states[CurrentState];
        _activeState.Enter();

        CurrentStateForDebug = CurrentState; // 디버그용
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_OrderChangeState(EPlayerState newState)
    {
        ChangeState(newState);
    }

    public void TakeDamage(int amount, PlayerRef attacker)
    {
        if (!HasStateAuthority) return;

        var dmg = amount * (100 / (100 + _controller.Stat.GetStat(EStatType.Defense)));

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

    public void RequestChangeState(EPlayerState newState)
    {
        ChangeState(newState);
    }

    //private void SpawnCorpse()
    //{
    //    var playerName = _controller.gameObject.name;
    //    ItemManager.Instance.RPC_CreateCorpseObject(playerName, _controller.transform.position, Quaternion.identity);
    //}

}