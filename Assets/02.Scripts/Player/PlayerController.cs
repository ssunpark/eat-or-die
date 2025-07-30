
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

public class StateValues
{
    public IInteractable Interactable;
    public IUsable Usable;
    public float MoveHungerTimer = 0f;
    public float MoveHungerInterval = 1f;
}

public class FSMStateInstances
{
    public PlayerIdleState Idle;
    public PlayerMoveState Move;
    public PlayerAttackState Attack;
    public PlayerUseItemState UseItem;
    public PlayerInteractState Interact;
    public PlayerHitState Hit;
    public PlayerDeadState Dead;
    public PlayerCookingState Cooking;
    public PlayerBerserkState Berserk;
}
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : CharacterBase, IStateMachineOwner
{
    #region Networked Properties

    [Networked]
    public bool MoveFlag { get; set; }

    #endregion

    #region FSM

    private StateMachine<APlayerStateBase> _playerFSM; // 메인 상태 머신
    public StateMachine<APlayerStateBase> FSM => _playerFSM;
    public FSMStateInstances FSMStateInstances { get; private set; }
    public StateValues StateValues { get; set; } = new StateValues();

    #endregion

    #region Components & References

    private NetworkCharacterController _characterController;
    [HideInInspector] public PlayerAnimator PlayerAnimatorController;
    [HideInInspector] public PlayerInteractions Interact;
    public PlayerMove Movement;

    #endregion

    #region Hunger / Resource

    private float _prevHunger = 0f;
    private HungerEffectHandler _hungerEffectHandler;
    public HungerEffectHandler HungerEffectHandler => _hungerEffectHandler;

    #endregion

    #region Attack Tracking

    private float _lastAttackTime;
    public float LastAttackTime
    {
        get => _lastAttackTime;
        set => _lastAttackTime = value;
    }

    #endregion

    #region HUD

    [SerializeField]
    private string _playerHUDTagName = "PlayerHUD";

    #endregion

    #region State Flags

    private bool _isSpawned = false;

    #endregion

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetMoveFlag(bool flag)
    {
        MoveFlag = flag;
    }

    public override void FixedUpdateNetwork()
    {
        Stat.UpdateStats(Runner.DeltaTime);
    }

    private void Awake()
    {
        InitializeFSM();
    }

    private void InitializeFSM()
    {
        FSMStateInstances = new FSMStateInstances
        {
            Idle = new PlayerIdleState(this),
            Move = new PlayerMoveState(this),
            Attack = new PlayerAttackState(this),
            UseItem = new PlayerUseItemState(this),
            Interact = new PlayerInteractState(this),
            Hit = new PlayerHitState(this),
            Dead = new PlayerDeadState(this),
            Cooking = new PlayerCookingState(this),
            Berserk = new PlayerBerserkState(this)
        };

        _playerFSM = new StateMachine<APlayerStateBase>("PlayerFSM",
            FSMStateInstances.Idle,
            FSMStateInstances.Move,
            FSMStateInstances.Attack,
            FSMStateInstances.UseItem,
            FSMStateInstances.Interact,
            FSMStateInstances.Hit,
            FSMStateInstances.Dead,
            FSMStateInstances.Cooking,
            FSMStateInstances.Berserk
        );
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Room.Instance.SetLocalPlayer(gameObject);

            var camera = Camera.main.GetComponent<FollowCamera>();
            if (camera != null)
            {
                Transform followTarget = transform;
                camera.SetTarget(followTarget);
            }
        }
        //관전 모드에서 플레이어를 등록
        //SpectatorManager.Instance?.RegisterPlayer(this);
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        _hungerEffectHandler = new HungerEffectHandler(Resource, Stat);
        Interact = GetComponent<PlayerInteractions>();
        Movement = GetComponent<PlayerMove>();
        _isSpawned = true;
        TryInitialize();
    }

    private void OnDestroy()
    {
        //관전 모드에서 플레이어를 제거
        //SpectatorManager.Instance?.UnregisterPlayer(this);
    }

    private void TryInitialize()
    {
        if (_isSpawned)
        {
            _characterController.maxSpeed = Stat.GetStat(EStatType.MoveSpeed);
            _characterController.jumpImpulse = Stat.GetStat(EStatType.JumpPower);
            _characterController.acceleration = Stat.GetStat(EStatType.Acceleration);

            if (Object.HasInputAuthority)
            {
                InitializePlayerHUD();
            }

            if (TryGetComponent(out PlayerMove playerMove))
            {
                playerMove.Initialize(Stat, _characterController, this, Resource);
            }
            else
            {
                Debug.LogError("PlayerMove component not found!!");
            }

        }

        
        _playerFSM.SetDefaultState((int)EPlayerState.Idle);
        Resource.OnHungerChanged += EvaluateCurrentHunger;
    }


    public void CollectStateMachines(List<IStateMachine> list)
    {
        list.Add(_playerFSM);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_PlayAnimTrigger(EAnimTrigger trigger)
    {
        PlayerAnimatorController.PlayTrigger(trigger);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_DealDamage(NetworkObject target, float amount)
    {
        if (target == null || !target.HasStateAuthority) return;

        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(amount, Object.InputAuthority);
        }
    }
    public void TakeDamage(float amount, PlayerRef attacker)
    {
        if (!HasStateAuthority) return;

        float defense = Stat.GetStat(EStatType.Defense);
        float finalDmg = amount * (100 / (100 + defense));

        Resource.ConsumeHunger(finalDmg);
        _playerFSM.ForceActivateState(FSMStateInstances.Hit);
    }

    private void InitializePlayerHUD()
    {
        // 나중에 UIManager를 통해 HUD를 관리할 예정
        GameObject hudObject = GameObject.FindGameObjectWithTag(_playerHUDTagName);
        if (hudObject != null)
        {
            UI_HUDPlayerHP hudHP = hudObject.GetComponent<UI_HUDPlayerHP>();
            if (hudHP != null)
            {
                hudHP.Initialize(Resource, Stat); // ResourceManager와 StatManager 전달
            }
            else
            {
                Debug.LogError($"HUD 오브젝트 '{_playerHUDTagName}'에 UI_HUDPlayerHP 스크립트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"씬에서 태그 '{_playerHUDTagName}'를 가진 HUD 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void EvaluateCurrentHunger(float current, float max)
    {

        if( current <= 0)
        {
            _playerFSM.ForceActivateState(FSMStateInstances.Dead);
        }
        else if (current < max * 0.1f && _prevHunger > current && _playerFSM.ActiveState != FSMStateInstances.Berserk)
        {
            _playerFSM.ForceActivateState(FSMStateInstances.Berserk);
        }
        else if(_prevHunger < current && current >= max * 0.1f && _playerFSM.ActiveState == FSMStateInstances.Berserk)
        {
            _playerFSM.ForceActivateState(FSMStateInstances.Idle);
        }

        _prevHunger = current;
    }

}
