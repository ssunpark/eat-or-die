//using Fusion;
//using Fusion.Addons.FSM;
//using UnityEngine;
//using System.Collections.Generic;

//// 현재 플레이어 상태 전환 관리
//public class PlayerStateMachine : NetworkBehaviour, IDamageable, IStateMachineOwner
//{
//    private StateMachine<APlayerStateBase> _fsm;
//    private PlayerController _controller;
//    [SerializeField] private bool _logStates = false;
//    public override void Spawned()
//    {
//        _controller = GetComponent<PlayerController>();

//        // FSM 상태 인스턴스 생성
//        _controller.FSMStateInstances = new FSMStateInstances
//        {
//            Idle = new PlayerIdleState(_controller),
//            Move = new PlayerMoveState(_controller),
//            Attack = new PlayerAttackState(_controller),
//            UseItem = new PlayerUseItemState(_controller),
//            Interact = new PlayerInteractState(_controller),
//            Hit = new PlayerHitState(_controller),
//            Dead = new PlayerDeadState(_controller),
//            Cooking = new PlayerCookingState(_controller),
//            Berserk = new PlayerBerserkState(_controller)
//        };

//        _fsm = new StateMachine<APlayerStateBase>("PlayerFSM",
//            _controller.FSMStateInstances.Idle,
//            _controller.FSMStateInstances.Move,
//            _controller.FSMStateInstances.Attack,
//            _controller.FSMStateInstances.UseItem,
//            _controller.FSMStateInstances.Interact,
//            _controller.FSMStateInstances.Hit,
//            _controller.FSMStateInstances.Dead,
//            _controller.FSMStateInstances.Cooking,
//            _controller.FSMStateInstances.Berserk
//        );

//        _fsm.EnableLogging = _logStates;
//        _fsm.SetDefaultState(_controller.FSMStateInstances.Idle.StateId);

//        SubscribeBerserkTrigger();

//        _controller.Resource.OnHungerChanged += OnHungerChanged;
//    }
//    public override void FixedUpdateNetwork()
//    {
//    }

//    public override void Despawned(NetworkRunner runner, bool hasState)
//    {
//        _controller.Resource.OnHungerChanged -= OnHungerChanged;
//        _fsm?.Deinitialize(hasState);
//    }
//}





////public bool IsBerserk => _isBerserk;
////private bool _isBerserk = false;
////public void SetBerserk(bool value)
////{
////    _isBerserk = value;
////}

////#region References
////[HideInInspector] public PlayerInteractions Interact;
////private PlayerController _controller;
////#endregion

////public IInteractable Interactable;
////public IUsable Usable;

//////이동중일 때 배고픔 감소 속도 조절을 위한 타이머
////[HideInInspector]
////public float MoveSatietyTimer;
////// 몇초 이동했을 때 배고픔 감소가 일어날지
////public float MoveStatietyInterval = 1f;


////public override void Spawned()
////{
////    _controller = GetComponent<PlayerController>();
////    Interact = GetComponent<PlayerInteractions>();
////    _states = new Dictionary<EPlayerState, APlayerStateBase>
////    {
////        { EPlayerState.Idle, new PlayerIdleState(this, _controller) },
////        { EPlayerState.Move, new PlayerMoveState(this, _controller) },
////        { EPlayerState.Attack, new PlayerAttackState(this, _controller) },
////        { EPlayerState.Hit, new PlayerHitState(this, _controller) },
////        { EPlayerState.UsingTool, new PlayerUseItemState(this, _controller) },
////        { EPlayerState.Cooking, new PlayerCookingState(this, _controller) },
////        { EPlayerState.Interact, new PlayerInteractState(this, _controller) },
////        { EPlayerState.Berserk, new PlayerBerserkState(this, _controller) },
////        /*{ EPlayerState.Down, new PlayerDownState(this, _controller) },

////         */
////    };

////    CurrentState = EPlayerState.Idle;
////    _activeState = _states[CurrentState];
////    _activeState.Enter();

////    _controller.Resource.OnHungerChanged += Resource_OnSatietyChanged;
////    SubscribeBerserkTrigger();
////}

////public override void Despawned(NetworkRunner runner, bool hasState)
////{
////    _controller.Resource.OnHungerChanged -= Resource_OnSatietyChanged;
////    _activeState?.Exit();
////    _activeState = null;
////    _states.Clear();
////    _states = null;
////}

////private void Resource_OnSatietyChanged(float current, float max)
////{
////    var ratio = current / max;
////    if (ratio < 0.1f) SetBerserk(true);
////    else SetBerserk(false);
////    if (current <= 0)
////    {
////        ChangeState(EPlayerState.Dead);
////    }
////}

////public override void FixedUpdateNetwork()
////{
////    _activeState?.Tick();
////}

////public void ChangeState(EPlayerState newState)
////{
////    if (newState == CurrentState) return;
////    CurrentState = newState;
////    _activeState?.Exit();
////    _activeState = _states[CurrentState];
////    _activeState.Enter();

////    CurrentStateForDebug = CurrentState; // 디버그용
////}

////[Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
////public void RPC_ForceChangeState(EPlayerState newState)
////{
////    ChangeState(newState);
////}
////public void RequestChangeState(EPlayerState newState)
////{
////    if (HasStateAuthority)
////    {
////        RPC_ForceChangeState(newState); // 모든 클라에게 알림
////    }
////    else if (HasInputAuthority)
////    {
////        ChangeState(newState); // 본인 클라에서 직접 처리
////    }
////}
////public void TakeDamage(int amount, PlayerRef attacker)
////{
////    if (!HasStateAuthority) return;

////    var dmg = amount * (100 / (100 + _controller.Stat.GetStat(EStatType.Defense)));

////    _controller.Resource.ConsumeHunger(dmg);
////    RequestChangeState(EPlayerState.Hit);
////}

////public bool CanMove
////{
////    get
////    {
////        if (_states.TryGetValue(CurrentState, out var state))
////        {
////            return state.CanMove;
////        }
////        return false;
////    }
////}

//////private void SpawnCorpse()
//////{
//////    var playerName = _controller.gameObject.name;
//////    ItemManager.Instance.RPC_CreateCorpseObject(playerName, _controller.transform.position, Quaternion.identity);
//////}

////private void SubscribeBerserkTrigger()
////{
////    _controller.Stat.RegisterModifierCallback(
////        EStatType.MoveSpeed,
////        (statType, modifier) =>
////        {
////            if (modifier.Source?.ToString() == "Hungry_Critical")
////            {
////                Debug.Log("Berserk Triggered: " + modifier.Source);
////                if (HasInputAuthority)
////                    ChangeState(EPlayerState.Berserk);
////            }
////            else
////            {
////                Debug.Log(modifier.Source + " is not Hungry_Critical, current state: " + CurrentState);
////            }
////        },
////        (statType, modifier) =>
////        {
////            if (modifier.Source?.ToString() == "Hungry_Critical" && CurrentState == EPlayerState.Berserk)
////                if (HasInputAuthority)
////                    ChangeState(EPlayerState.Idle);
////        });
////}