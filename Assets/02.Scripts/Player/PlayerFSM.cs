using System.Collections.Generic;
using EPOOutline;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;


#if UNITY_EDITOR
#endif

public enum EUseItemMode : byte
{
    Self = 0,
    Give = 1,
    Targeted = 2
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
    public PlayerCorpseState Corpse;
}
[RequireComponent(typeof(StateMachineController))]
public class PlayerFSM : NetworkBehaviour, IStateMachineOwner
{
    public bool EnableDebugLog = false;
    #region FSM

    private StateMachine<APlayerStateBase> _playerFSM; // 메인 상태 머신
    public StateMachine<APlayerStateBase> StateMachine => _playerFSM;
    public FSMStateInstances FSMStateInstances { get; private set; }


    #endregion

    #region Components & References
    public PlayerAnimator PlayerAnimatorController { get; private set; }
    public PlayerInteractions Interact { get; private set; }
    public PlayerItemHolder ItemHolder { get; private set; }
    public CharacterStatNetworkSync StatNetworkSync { get; private set; }

    public LayerMask attackableLayerMask;

    #endregion

    private float _lastAttackTime;
    public float LastAttackTime
    {
        get => _lastAttackTime;
        set => _lastAttackTime = value;
    }


    public Player PlayerNetworkObject;
    [Networked]
    public NetworkBool CanInteract { get; set; } = false;
    [Networked]
    public NetworkBool CanAttack { get; set; } = true;
    [Networked]
    public NetworkBool CanUseItem { get; set; } = false;

    [Networked]
    public NetworkBool IsDead { get; set; } = false;

    [Networked]
    public NetworkObject ItemUseTarget { get; set; } = null;

    [Networked]
    public NetworkObject InteractTarget { get; set; } = null;

    Collider[] _testColliders = new Collider[8];
    public LayerMask InteractLayerMask;
    public LayerMask BerserkLayerMask;
    public SimpleKCC SimpleKCC;
    private NetworkInputData _currentInput;
    private NetworkInputData _previousInput;
    public ParticleSystem HungryEffect;
    private float _floaterTime = 1f;
    public NetworkInputData CurrentInput => _currentInput;
    public NetworkInputData PreviousInput => _previousInput;

    public const float INTERACTABLE_DISTANCE = 3f;
    public const float MAX_RAYCAST_DISTANCE = 100f;
    private const float _useItemMaxDistance = 2.0f;
    [SerializeField] private GameObject _reviveSelectUIPrefab;
    public GameObject HeadCanvas;
    private Transform _uiParent;

    [SerializeField] UI_UseOrInteract _useUI;
    [SerializeField] UI_UseOrInteract _interactUI;
    [Networked]
    public EUseItemMode UseItemMode { get; set; } = EUseItemMode.Self;

    [Networked, Capacity(8)]
    public NetworkLinkedList<NetworkObject> HitTargets { get; }

    public void CollectStateMachines(List<IStateMachine> list)
    {
        InitializeFSM();
        list.Add(_playerFSM);
    }

    public void ShowSelectPanel()
    {
        Instantiate(_reviveSelectUIPrefab, _uiParent)
            .GetComponent<UI_ReviveSelect>()
            .Initialize(this,PlayerNetworkObject);
    }

    public override void Spawned()
    {
        SimpleKCC = GetComponent<SimpleKCC>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        Interact = GetComponent<PlayerInteractions>();
        StatNetworkSync = GetComponent<CharacterStatNetworkSync>();
        ItemHolder = GetComponent<PlayerItemHolder>();
        PlayerNetworkObject = GetComponent<Player>();
        if (Object.HasInputAuthority)
        {
            _uiParent = GameObject.FindGameObjectWithTag("UIParent")?.transform;
            GetComponentInChildren<OutlineController>().enabled = false;
            GetComponentInChildren<Outlinable>().enabled = false;
        }
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
            Recover = new PlayerRecoverState(this),
            Corpse = new PlayerCorpseState(this)
        };

        _playerFSM = new StateMachine<APlayerStateBase>("Player FSM",
            FSMStateInstances.Idle,
            FSMStateInstances.Move,
            FSMStateInstances.Attack, 
            FSMStateInstances.Interact,
            FSMStateInstances.UseItem, 
            FSMStateInstances.Cooking,
            FSMStateInstances.Hit,
            FSMStateInstances.Dead,
            FSMStateInstances.Berserk,
            FSMStateInstances.Recover,
            FSMStateInstances.Corpse
        );
    }

    public void ResetOutlinesAndTags()
    {
        _interactUI.TargetObject = null;
        _useUI.TargetObject = null;
        InteractTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);
        ItemUseTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);
    }
    float _timer = 0f;
    public override void FixedUpdateNetwork()
    {
        if (PlayerNetworkObject == null || PlayerNetworkObject.Resource == null)
        {
            return;
        }
        if (HasStateAuthority)
        {
            if (IsDead)
            {
                return;
            }
            
            float HungerRecoveryOverTime = PlayerNetworkObject.Stat.GetStat(EStatType.HungerRecoveryOverTime);

            if(HungerRecoveryOverTime != 0f)
            {
                _timer += Runner.DeltaTime;
                if (_timer >= _floaterTime)
                {
                    _timer = 0f;
                    if (PlayerNetworkObject.Resource.GetHungerPercent() == 1f) return;
                    PlayerNetworkObject.Resource.RestoreHunger(HungerRecoveryOverTime);
                    if (HungerRecoveryOverTime > 0f)
                    {
                        ParticleManager.Instance.DamageSpawn(HungerRecoveryOverTime, transform.position + Vector3.up * 0.5f, EDamageFloaterType.Heal, true);
                    }
                    else
                    {
                        ParticleManager.Instance.DamageSpawn(HungerRecoveryOverTime, transform.position + Vector3.up * 0.5f, EDamageFloaterType.Damage, true);
                    }
                }
            }

        }
        if (HasInputAuthority)
        {
            if (CanInteract)
            {
                if (!TestInteraction(CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.Interact)))
                {
                    InteractTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);
                    _interactUI.TargetObject = null;
                    RPC_SetInteractTarget(null);
                }
            }

            if (CanUseItem)
            {
                if (!TestUseItem(CurrentInput.buttons.WasPressed(PreviousInput.buttons, EButtons.UseItem)))
                {
                    ItemUseTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);
                    _useUI.TargetObject = ItemUseTarget?.gameObject;
                    RPC_SetItemUseTargetAndMode(null, EUseItemMode.Self);
                }
            }
        }

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetInteractTarget(NetworkObject obj)
    {
        InteractTarget = obj;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_SetItemUseTargetAndMode(NetworkObject obj, EUseItemMode mode)
    {
        ItemUseTarget = obj;
        UseItemMode = mode;
    }


    private bool TestUseItem(bool usePressed)
    {
        if (ItemHolder.HeldItemInstance == null)
            return false;

        string requiredTag = ItemHolder.InteractionTag;
        if (string.IsNullOrEmpty(requiredTag) || requiredTag == "Unarmed")
            return false;

        // ▶ Player 대상: 기존 로직 유지(커서 우선 + 자기 자신 fallback)
        if (requiredTag == "Player")
        {
            if (TryGetPlayerUnderCursor(out var playerUnderCursor))
            {
                if (ItemUseTarget != playerUnderCursor)
                    ItemUseTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);

                RPC_SetItemUseTargetAndMode(playerUnderCursor, EUseItemMode.Give);
                playerUnderCursor.GetComponent<OutlineController>()?.SetOutlineActive(true);
                _useUI.TargetObject = ItemUseTarget?.gameObject;
                return true;
            }

            // 커서에 유효한 다른 플레이어가 없으면 자기 자신
            ItemUseTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);


            RPC_SetItemUseTargetAndMode(PlayerNetworkObject.Object, EUseItemMode.Self);
            return true;
        }

        // ▶ 그 외 태그: 커서가 가리키는 오브젝트 기반
        if (TryGetTaggedObjectUnderCursor(requiredTag, _useItemMaxDistance, out var net))
        {
            if (ItemUseTarget != net)
                ItemUseTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);

            RPC_SetItemUseTargetAndMode(net, EUseItemMode.Targeted);
            net.GetComponent<OutlineController>()?.SetOutlineActive(true);
            _useUI.TargetObject = ItemUseTarget?.gameObject;
            return true;
        }
        _useUI.TargetObject = null;
        return false;
    }

    private bool TestInteraction(bool interactPressed)
    {
        if (!InputReader.Instance.HaveControl())
            return false;

        if (!TryGetInteractableUnderCursor(INTERACTABLE_DISTANCE, out var interactable, out var net))
            return false;

        if (interactPressed && interactable.IsImmediate)
        {
            interactable.Interact(); // 로컬 즉시형 처리
            return false;
        }

        // 즉시형이 아니면 하이라이트 + 서버에 타깃 통지
        var comp = (Component)interactable;

        if (InteractTarget != net)
            InteractTarget?.GetComponent<OutlineController>()?.SetOutlineActive(false);

        _interactUI.TargetObject = comp.gameObject;

        comp.GetComponent<OutlineController>()?.SetOutlineActive(true);
        RPC_SetInteractTarget(net);
        return true;
    }



    public void SetInput(NetworkInputData input)
    {
        _previousInput = _currentInput;
        _currentInput = input;
    }

    
    public void RequestActivateState(EPlayerState state)
    {
        if (HasStateAuthority)
        {
            _playerFSM.ForceActivateState((int)state);
        }
        else if(HasInputAuthority)
        {
                RPC_RequestChangeStates(state);
        }
        
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestChangeStates(EPlayerState state)
    {
        _playerFSM.ForceActivateState((int)state);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_GrantExpOrderWithAmount([RpcTarget] PlayerRef player, string actionName, int amount)
    {
        PlayerNetworkObject.ExpHandler.GrantExp(actionName, amount);
        if (actionName == "KillMonster")
        {
            ParticleManager.Instance.DamageSpawn(amount, transform.position + Vector3.up * 0.5f, EDamageFloaterType.Experience, false);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_GrantExpOrder([RpcTarget] PlayerRef player, string actionName)
    {
        PlayerNetworkObject.ExpHandler.GrantExp(actionName);
    }
    private bool TryGetPlayerUnderCursor(out NetworkObject targetPlayer)
    {
        targetPlayer = null;
        var cam = Camera.main;
        if (cam == null) return false;

        var ray = cam.ScreenPointToRay(Input.mousePosition);
        var scene = Runner.GetPhysicsScene();
#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * MAX_RAYCAST_DISTANCE, Color.red, 1f);
#endif
        if (EnableDebugLog)
        {
            Debug.Log($"[PlayerFSM] Raycasting from {ray.origin} in direction {ray.direction} for {MAX_RAYCAST_DISTANCE} units.");
        }
        if (scene.Raycast(ray.origin, ray.direction, out RaycastHit hit, MAX_RAYCAST_DISTANCE, InteractLayerMask, QueryTriggerInteraction.Collide))
        {
            var go = hit.collider.gameObject;
            if (EnableDebugLog)
            {
                Debug.Log($"[PlayerFSM] Hit object: {go.name} at distance {hit.distance}");
            }
            var player = hit.collider.GetComponentInParent<PlayerFSM>();
            if (player == null) return false;

            if (player.IsDead) return false;

            var netObj = player.Object;
            if (netObj == null) return false;

            if (EnableDebugLog)
            {
                Debug.Log($"[PlayerFSM] NetworkObject found: {netObj.name}");
            }
            var dist = Vector3.Distance(transform.position, netObj.transform.position);
            if (EnableDebugLog)
            {
                Debug.Log($"[PlayerFSM] Distance to player: {dist}");
            }
            if (dist <= _useItemMaxDistance)
            {
                if (EnableDebugLog)
                {
                    Debug.Log($"[PlayerFSM] Player is within use item distance: {dist}");
                }
                if (netObj == Object)
                {
                    if (EnableDebugLog)
                    {
                        Debug.Log($"[PlayerFSM] target is me");    
                    }
                    _useUI.TargetObject = netObj.gameObject;
                    _useUI.ActionName = "먹기";
                    return false;
                }
                targetPlayer = netObj;
                _useUI.TargetObject = netObj.gameObject;
                return true;
            }
        }
        if (EnableDebugLog)
        {
            Debug.Log($"[PlayerFSM] No player found under cursor.");
        }
        return false;
    }
    private bool TryGetHitUnderCursor(out RaycastHit hit)
    {
        hit = default;
        var cam = Camera.main;
        if (cam == null) return false;

        var ray = cam.ScreenPointToRay(Input.mousePosition);
#if UNITY_EDITOR
        if (EnableDebugLog)
            Debug.DrawRay(ray.origin, ray.direction * MAX_RAYCAST_DISTANCE, Color.cyan, 0.1f);
#endif
        var scene = Runner.GetPhysicsScene();
        return scene.Raycast(ray.origin, ray.direction, out hit, MAX_RAYCAST_DISTANCE, InteractLayerMask, QueryTriggerInteraction.Collide);
    }

    private bool TryGetTaggedObjectUnderCursor(string requiredTag, float maxDistance, out NetworkObject net)
    {
        net = null;
        if (!TryGetHitUnderCursor(out var hit)) return false;

        var go = hit.collider.gameObject;
        // 태그 확인 (자식 콜라이더일 수 있으니 root까지 확인)
        var tagged = go.CompareTag(requiredTag) ? go : go.transform.root.gameObject;
        if (!tagged.CompareTag(requiredTag)) return false;

        // 거리 확인 (플레이어 기준)
        if (Vector3.Distance(transform.position, tagged.transform.position) > maxDistance) return false;

        // NetworkObject 구득 (부모 포함)
        if (!tagged.TryGetComponent(out net))
            net = tagged.GetComponentInParent<NetworkObject>();

        return net != null;
    }

    private bool TryGetInteractableUnderCursor(float maxDistance, out IInteractable interactable, out NetworkObject net)
    {
        interactable = null;
        net = null;

        if (!TryGetHitUnderCursor(out var hit)) { 
            if( EnableDebugLog )
                Debug.Log("[PlayerFSM] No hit detected under cursor for interaction.");
            return false; }
        interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable == null)
        {
            if (EnableDebugLog)
                Debug.Log("[PlayerFSM] No interactable component found on hit object.");
            return false;
        }
        
        var comp = (Component)interactable;
        float distanceoffset = interactable.InteractionDistanceOffset;
        if (Vector3.Distance(transform.position, comp.transform.position) > maxDistance + distanceoffset)
        {
            if (EnableDebugLog)
                Debug.Log($"[PlayerFSM] Interactable {comp.name} is too far away ({Vector3.Distance(transform.position, comp.transform.position)} > {maxDistance}).");
            return false;
        }

        comp.TryGetComponent(out net);
        if (net == null) net = comp.GetComponentInParent<NetworkObject>();

        return net != null;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestApplyStatModifier(EStatType statType, EStatModifierType modType, float value, float duration, string source, RpcInfo info = default)
    {
        if (!HasStateAuthority) return;
        if (Object.InputAuthority != info.Source) return;

        var mod = new StatModifier(modType, value, source, duration > 0f, duration);
        PlayerNetworkObject.Stat.ApplyModifier(statType, mod);

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestUseFood(int foodId, NetworkObject target, RpcInfo info = default)
    {
        if (!HasStateAuthority) return;

        var targetPlayer = target?.GetComponent<Player>();
        if (targetPlayer == null) return;

        var list = FoodDB.Instance.Get(foodId);
        if (list == null || list.Count == 0) return;

        foreach (var e in list)
        {
            var mod = new StatModifier(e.Op, e.Value, foodId, e.Duration > 0f, e.Duration);
            targetPlayer.Stat.ApplyModifier(e.Stat, mod);
        }
    }

    [Rpc(Fusion.RpcSources.InputAuthority, Fusion.RpcTargets.StateAuthority,
     HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_RequestUseFoodOnTarget(int foodId, NetworkObject target, RpcInfo info = default)
    {
        if (!HasStateAuthority) return;

        var targetPlayer = target ? target.GetComponent<Player>() : null;
        if (targetPlayer == null) return;

        var list = FoodDB.Instance.Get(foodId);
        if (list == null || list.Count == 0) return;

        foreach (var e in list)
        {
            var mod = new StatModifier(e.Op, e.Value, foodId, e.Duration > 0f, e.Duration);
            targetPlayer.Stat.ApplyModifier(e.Stat, mod); // ✅ 대상에게 적용
        }
    }

}
