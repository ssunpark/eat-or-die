
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

public class StateValues
{
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
    public PlayerRecoverState Recover;
}
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
[RequireComponent(typeof(StateMachineController))]
public class PlayerFSM : CharacterBase, IStateMachineOwner
{
    #region Networked Properties

    [Networked, OnChangedRender(nameof(MoveChanged))]
    public bool MoveFlag { get; set; }

    [Networked, OnChangedRender(nameof(MoveChanged))] public bool IsMoving { get; set; } = false;

    #endregion

    #region FSM

    private StateMachine<APlayerStateBase> _playerFSM; // 메인 상태 머신
    public StateMachine<APlayerStateBase> StateMachine => _playerFSM;
    public FSMStateInstances FSMStateInstances { get; private set; }
    public StateValues StateValues { get; set; } = new StateValues();

    private StateChangeRequestQueue<APlayerStateBase> _stateRequestQueue = new StateChangeRequestQueue<APlayerStateBase>();

    #endregion

    #region Components & References

    private NetworkCharacterController _characterController;
    public PlayerAnimator PlayerAnimatorController { get; private set; }
    public PlayerInteractions Interact {  get; private set; }
    public PlayerItemHolder ItemHolder { get; private set; }
    public PlayerMove Movement { get; private set; }
    public CharacterStatNetworkSync StatNetworkSync { get; private set; }

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


    public Player PlayerNetworkObject;
    [Networked]
    public NetworkBool CanInteract { get; set; } = false;
    [Networked]
    public NetworkBool CanAttack { get; set; } = true;
    [Networked]
    public NetworkBool CanUseItem { get; set; } = false;

    Collider[] _testColliders = new Collider[8];
    public LayerMask InteractLayerMask;
    //public SimpleKCC simpleKCC;

    public const float INTERACTABLE_DISTANCE = 2f;
    public const float MAX_RAYCAST_DISTANCE = 100f;
    private float _checkTimer;

    private GameObject _useTarget = null;
    public GameObject UseTarget => _useTarget;
    private GameObject _interactTarget = null;
    public GameObject InteractTarget => _interactTarget;
    private void Update()
    {
        if (!HasInputAuthority) return;

        _checkTimer += Time.deltaTime;
        if (_checkTimer > 0.1f)
        {
            _checkTimer = 0f;
            bool canUseItem = RaycastCheckForUsableItem(out _useTarget);
            bool canInteract = RaycastCheckForInteractable(out _interactTarget);

            RPC_SetFlags(canUseItem, canInteract);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetFlags(bool canUseItem, bool canInteract)
    {
        CanUseItem = canUseItem;
        CanInteract = canInteract;
    }

    public void SetMoveFlagNetwork(bool flag)
    {
        if (HasInputAuthority)
            RPC_SetMoveFlag_Input(flag);
        else if (HasStateAuthority)
            RPC_SetMoveFlag_State(flag);
    }

    private void MoveChanged()
    {
        PlayerAnimatorController.SetIsMoving(!MoveFlag && IsMoving);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetMoveFlag_Input(bool flag)
    {
        MoveFlag = flag;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetMoveFlag_State(bool flag)
    {
        MoveFlag = flag;
    }

    public override void FixedUpdateNetwork()
    {
        Stat.UpdateStats(Runner.DeltaTime);
        _stateRequestQueue.ExecuteAll(_playerFSM);
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
            Berserk = new PlayerBerserkState(this),
            Recover = new PlayerRecoverState(this)
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
        StatNetworkSync = GetComponent<CharacterStatNetworkSync>();
        ItemHolder = GetComponent<PlayerItemHolder>();
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

    public void PlayAnimTrigger(EAnimTrigger trigger)
    {
        PlayerAnimatorController.PlayTrigger(trigger);
    }

    public void PlayAnimTriggerNetwork(EAnimTrigger trigger)
    {
        // 본인 즉시 실행 (지연 없이)
        PlayAnimTrigger(trigger);

        // 모든 클라에 전파
        if (HasInputAuthority)
            RPC_PlayTrigger(trigger);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_PlayTrigger(EAnimTrigger trigger)
    {
        if (HasInputAuthority) return; // 이미 실행했음
        PlayAnimTrigger(trigger);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_DealDamage(NetworkObject target, float amount)
    {
        Debug.Log($"{target}");
        if (target == null) return;

        if (target.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(amount, Object.InputAuthority);
        }
    }
    public void TakeDamage(float amount, PlayerRef attacker)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        float defense = Stat.GetStat(EStatType.Defense);
        float finalDmg = amount * (100 / (100 + defense));
        RequestState(FSMStateInstances.Hit);
        Resource.ConsumeHunger(finalDmg);
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
        if(current/max > 0.2)
        {
            _prevHunger = current;
            return;
        }
        if (current <= 0)
        {
            RequestState(FSMStateInstances.Dead);
        }
        else if (current < max * 0.1f && _prevHunger > current)
        {
            if (_playerFSM.ActiveState != FSMStateInstances.Berserk)
            {
                RequestState(FSMStateInstances.Berserk);
            }
        }
        else if (_prevHunger < current && current >= max * 0.1f)
        {
            // Berserk 상태면 Recover로 진입 시도
            if (_playerFSM.ActiveState == FSMStateInstances.Berserk)
            {
                StateMachine.TryActivateState(FSMStateInstances.Recover);
            }
        }

        _prevHunger = current;
    }

    public void RequestState(APlayerStateBase nextState)
    {
        _stateRequestQueue.Request(nextState);
    }

    public void ForceOverrideState(APlayerStateBase nextState)
    {
        _stateRequestQueue.ForceOverride(nextState);
    }

    public bool RaycastCheckForUsableItem(out GameObject target)
    {
        target = null;
        if (ItemHolder.HeldItem == null)
            return false;

        string requiredTag = ItemHolder.InteractionTag;
        if (string.IsNullOrEmpty(requiredTag) || requiredTag == "Undefined")
        {
            Debug.Log($"[PlayerController] requiredTag is {requiredTag}.");
            // 사용아이템의 상호작용가능한 물체가 Untagged일 수도 있어서 임시로 Undefined일때 체크
            return false;
        }
        
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, MAX_RAYCAST_DISTANCE, LayerMask.GetMask("Interactable")))
        {
            Debug.Log($"[PlayerController] Raycast에서 검출된 오브젝트 없음.");
            return false;
        }

        GameObject hitObject = hit.collider.gameObject;
        if (!hitObject.CompareTag(requiredTag))
        {
            Debug.Log($"[PlayerController] hitObject: {hitObject.name}, {hitObject.tag}");
            return false;
        }

        float dist = Vector3.Distance(transform.position, hitObject.transform.position);
        if (dist > INTERACTABLE_DISTANCE)
        {
            Debug.Log($"[PlayerController] 거리 초과: {dist} > {INTERACTABLE_DISTANCE}");
            return false;
        }

        target = hitObject;
        Debug.Log($"[PlayerController] CanUseHeldItem 성공: {hitObject.name}, 거리: {dist}");
        return true;
    }
    public bool RaycastCheckForInteractable(out GameObject target)
    {
        target = null;

        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, MAX_RAYCAST_DISTANCE, LayerMask.GetMask("Interactable")))
            return false;

        GameObject hitObject = hit.collider.gameObject;

        float dist = Vector3.Distance(transform.position, hitObject.transform.position);
        if (dist > INTERACTABLE_DISTANCE)
        {
            return false;
        }

        target = hitObject;
        return true;
    }

}
