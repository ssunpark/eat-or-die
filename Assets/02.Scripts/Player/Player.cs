using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using RaycastPro.Detectors;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(RangeDetector))]
public class Player : CharacterBase, IAttackable
{
    [Serializable]
    public class InitialItemData
    {
        public int itemId;
        public int quantity;
        public float durability;
    }

    [SerializeField] private List<InitialItemData> InitialItems;

    public TraitExpHandler ExpHandler { get; private set; }
    public List<CharacterTraitData> TraitDataList { get; private set; }
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    [Networked] public TickTimer DamagedTimer { get; set; }


    [SerializeField] private GameObject _renderObject;
    [SerializeField] private GameObject _playerHeadUI;

    private CinemachineImpulseSource _impulseSource;

    [SerializeField] private UI_HeadPlayerHP _headHpBar;
    float _damageRecoveryTime = 1.2f;

    public PlayerFSM PlayerFSM;
    private bool _hasPlayerTrackerRef;

    //private PlayerTracker _playerTrackerRef;
    public PlayerItemHolder ItemHolder { get; private set; }

    private Dictionary<string, float> _animationClipLengths;

    private Animator _animator;
    public Animator Anim => _animator;
    bool _isReset;
    public SimpleKCC SimpleKCC { get; private set; }
    public SkillManager Skill { get; private set; }

    public void InitializeTraitSystem(List<CharacterTraitData> dataList, TraitExpHandler expHandler)
    {
        TraitDataList = dataList;
        ExpHandler = expHandler;
    }
    public IDictionary<string, float> AnimationClipLengths
    {
        get
        {
            if (_animationClipLengths == null)
            {
                _animationClipLengths = new Dictionary<string, float>();
                CacheAnimationLengths();
            }
            return _animationClipLengths;
        }
    }

    public NetworkObject NetworkObject => Object;
    public bool IsDead => Resource.CurrentHunger <= 0;

    public override void Spawned()
    {
        base.Spawned();

        // 기존 즉시 접근 로직 제거하고 비동기 초기화로 이관
        InitAfterSpawnAsync().Forget();

        // 나머지 캐시/레퍼런스는 그대로
        _animator = GetComponent<Animator>();
        PlayerFSM = GetComponent<PlayerFSM>();
        ItemHolder = GetComponent<PlayerItemHolder>();
        SimpleKCC = GetComponent<SimpleKCC>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        Skill = new SkillManager(this);
    }

    private bool _spawnInitDone;
    private async UniTaskVoid InitAfterSpawnAsync()
    {
        if (_spawnInitDone) return; // 중복 방지

        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Yield();
        // 1) Resource, Stat 준비까지 대기 (최대 5초)
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => Resource != null && Stat != null, cancellationToken: token)
                  .Timeout(TimeSpan.FromSeconds(5)).SuppressCancellationThrow()
        );

        if (Resource != null)
        {
            // 중복 구독 방지 후 구독
            Resource.OnHungerChanged -= EvaluateCurrentHunger;
            Resource.OnHungerChanged += EvaluateCurrentHunger;
        }
        else
        {
            Debug.LogWarning("[Player] Resource not ready after timeout; skipping hunger hook.");
        }

        // 2) 로컬 플레이어만 카메라 바인딩 시도
        if (Object.HasInputAuthority)
        {
            // Room 참조는 있는 경우에만
            if (Room.Instance != null)
                Room.Instance.SetLocalPlayer(gameObject);

            // Camera.main & FollowCamera 준비까지 대기 (최대 3초)
            await UniTask.WaitUntil(() =>
            {
                var cam = Camera.main;
                return cam != null && cam.GetComponent<FollowCamera>() != null;
            }, cancellationToken: token).Timeout(TimeSpan.FromSeconds(3)).SuppressCancellationThrow();

            TryBindFollowCamera(); // 가드 포함
        }

        // 3) Trait/HUD 초기화
        if (HasInputAuthority)
        {
            if (ExpHandler != null && TraitDataList != null)
            {
                LoadTraitsFromStorage();
                InitializePlayerHUD_Safe();
            }
            else
            {
                // 기존 코루틴 대신 UniTask 대기(최대 5초)
                await UniTask.WaitUntil(
                    () => ExpHandler != null && TraitDataList != null,
                    cancellationToken: token
                ).Timeout(TimeSpan.FromSeconds(5)).SuppressCancellationThrow();

                if (ExpHandler != null && TraitDataList != null)
                {
                    LoadTraitsFromStorage();
                    InitializePlayerHUD_Safe();
                }
                else
                {
                    Debug.LogWarning("[Player] Trait system not ready after timeout; HUD init deferred.");
                }
            }

            await UniTask.WaitUntil(() => RoomInfoManager.Instance.CurrentRoomInfo != null, cancellationToken: token).Timeout(TimeSpan.FromSeconds(5)).SuppressCancellationThrow();
            
            if(RoomInfoManager.Instance.CurrentRoomInfo != null)
                GetInitialItem();
            else
            {
                Debug.LogError("Fuckyou");
            }
        }
        _spawnInitDone = true;
    }

    public void GetInitialItem()
    {
        foreach(var itemData in InitialItems)
        {
            var item = ItemManager.Instance.GetItem(itemData.itemId);
            var inst = new ItemInstance(item, itemData.quantity, itemData.durability);
            UnifiedInventoryManager.Instance.AddItem(inst);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hide">true: 숨기기</param>
    /// <param name="includeUI">UI도 숨길것인지</param>
    public void HideCharacter(bool hide, bool includeUI = true)
    {
        if (_renderObject != null)
            _renderObject.SetActive(!hide);
        if (includeUI)
        {
            _playerHeadUI.SetActive(!hide);
        }
    }
    private void TryBindFollowCamera()
    {
        var mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("[Player] MainCamera not found.");
            return;
        }

        var follow = mainCam.GetComponent<FollowCamera>();
        if (follow == null)
        {
            Debug.LogWarning("[Player] FollowCamera not found on MainCamera.");
            return;
        }

        follow.SetTarget(transform);
    }

    private void InitializePlayerHUD_Safe()
    {
        var hudObject = GameObject.FindGameObjectWithTag("PlayerHUD");
        if (hudObject != null)
        {
            var hudHP = hudObject.GetComponentInChildren<UI_HUDPlayerHP>(true);
            if (hudHP != null && Resource != null && Stat != null)
                hudHP.Initialize(Resource, Stat);
        }

        if (_headHpBar != null && Resource != null && Stat != null)
            _headHpBar.InitializeHeadHpBar(Resource, Stat);
    }


    public void LoadTraitsFromStorage()
    {
        foreach (var data in Trait.GetTraitSnapshot())
        {
            ETraitType type = data.Key;
            int level = TraitLevelStorage.GetLevel(type);
            float exp = TraitLevelStorage.GetExperience(type);

            var trait = Trait.GetTrait(type); // 내부 딕셔너리에서 가져오기
            trait?.SetLevel(level);
            trait?.AddExp(exp);

            Trait.LoadAllSkillPoints(type);
        }

        Trait.ReapplyAllTraitEffects(TraitDataList);
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }

    public override void FixedUpdateNetwork()
    {
        if (PlayerFSM == null)
        {
            PlayerFSM = GetComponent<PlayerFSM>();
        }
        if (PlayerFSM == null || PlayerFSM.StateMachine == null)
        {
            return;
        }
        if (Resource == null || Stat == null)
        {
            return;
        }
        if (SimpleKCC == null)
        {
            SimpleKCC = GetComponent<SimpleKCC>();
            if (SimpleKCC == null)
            {
                Debug.LogError("[Player] SimpleKCC is not initialized.");
                return;
            }
        }
        Stat.UpdateStats(Runner.DeltaTime);

        if (_isTeleporting)
        {
            if (SimpleKCC.enabled)
            {
                SimpleKCC.SetPosition(_teleportPosition);
                _isTeleporting = false;
            }
        }
        if (HasInputAuthority)
        {
            if (GetInput<NetworkInputData>(out var input))
            {
                PlayerFSM.SetInput(input);
            }
        }
        else if (HasStateAuthority)
        {
            if (Runner.TryGetInputForPlayer<NetworkInputData>(Object.InputAuthority, out var input))
            {
                PlayerFSM.SetInput(input);
            }

        }
        if (HasStateAuthority)
        {
            if (_nextState != null)
            {
                PlayerFSM.StateMachine.ForceActivateState(_nextState);
                _nextState = null;
            }
        }
    }


    public void RequestState(EPlayerState state)
    {
        if (PlayerFSM.StateMachine.ActiveState.StateId != (int)state)
        {
            if (HasStateAuthority)
            {
                _nextState = PlayerFSM.StateMachine.GetState((int)state);
            }
            else
            {
                Debug.Log("[Client] Requesting state change to: " + state);
                Debug.Log($"[Client] input: {HasInputAuthority}, id: {Object.Id}");
                Rpc_RequestState(state);
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_RequestState(EPlayerState state)
    {
        Debug.Log("[Host] Requesting state change to: " + state);

        if (PlayerFSM.StateMachine.ActiveState.StateId != (int)state)
        {
            _nextState = PlayerFSM.StateMachine.GetState((int)state);
        }
    }

    bool _takedDamage = false;
    float _prevHunger;

    APlayerStateBase _nextState = null;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_HealOrDamageFromEat(float amount)
    {
        if (amount > 0)
        {
            Resource.RestoreHunger(amount);
            ParticleManager.Instance.DamageSpawn(amount, transform.position + (Vector3.up * 0.5f), EDamageFloaterType.Heal, true);
            ParticleManager.Instance.PlayByKey("Use_Success_Eat", transform.position + (Vector3.up * 0.5f), Quaternion.identity, true);
            // 힐
        }
        else if (amount < 0)
        {
            Resource.ConsumeHunger(-amount);
            ParticleManager.Instance.DamageSpawn(-amount, transform.position + (Vector3.up * 0.5f), EDamageFloaterType.Damage, true);
            ParticleManager.Instance.PlayByKey("Use_Fail_Eat", transform.position + (Vector3.up * 0.5f), Quaternion.identity, true);
            // 데미지
        }
        else
        {
            Debug.Log("미친놈아 0을 왜 호출해");
        }
    }

    public void TryHealOrDamageFromEat(float amount)
    {
        RPC_HealOrDamageFromEat(amount);
    }

    private void EvaluateCurrentHunger(float current, float max)
    {
        if (_takedDamage)
        {
            _takedDamage = false;
            _prevHunger = current;

            if (current > 0)
            {
                if (PlayerFSM.StateMachine.ActiveState is not PlayerHitState)
                {
                    _nextState = PlayerFSM.StateMachine.GetState<PlayerHitState>();
                    return;
                }
            }
            else if (PlayerFSM.StateMachine.ActiveState is not PlayerDeadState)
            {
                _nextState = PlayerFSM.StateMachine.GetState<PlayerDeadState>();
                return;
            }
        }

        if (current / max > 0.2)
        {
            if (PlayerFSM.StateMachine.ActiveState is PlayerBerserkState)
            {
                if (PlayerFSM.EnableDebugLog)
                {
                    Debug.Log("[Player] Exiting Berserk State due to hunger recovery.");
                }
                _nextState = PlayerFSM.StateMachine.GetState<PlayerRecoverState>();
                _prevHunger = current;
                return;
            }
        }

        if (current <= 0)
        {
            if (PlayerFSM.StateMachine.ActiveState is not PlayerDeadState)
            {
                _nextState = PlayerFSM.StateMachine.GetState<PlayerDeadState>();
                return;
            }
        }
        else if (current < max * 0.1f)
        {
            if (PlayerFSM.StateMachine.ActiveState is not PlayerBerserkState)
            {
                if (PlayerFSM.EnableDebugLog)
                    Debug.Log("[Player] Entering Berserk State due to low hunger.");
                _nextState = PlayerFSM.StateMachine.GetState<PlayerBerserkState>();
                _prevHunger = current;
                return;
            }
        }
        else if (_prevHunger < current && current >= max * 0.1f)
        {
            if (PlayerFSM.StateMachine.ActiveState is PlayerBerserkState)
            {
                if (PlayerFSM.EnableDebugLog)
                {
                    Debug.Log("[Player] Exiting Berserk State due to hunger recovery.");
                }
                _nextState = PlayerFSM.StateMachine.GetState<PlayerRecoverState>();
                _prevHunger = current;
                return;
            }
        }
    }



    public override void Render()
    {
        if (Resource == null) return;
        if (Resource.CurrentHunger <= 0)
        {
            //damageToggleObject.SetActive(false);
            return;
        }
        if (!DamagedTimer.ExpiredOrNotRunning(Runner))
        {
            //damageToggleObject.SetActive((Runner.Tick % damageToggleObjectRate) < (damageToggleObjectRate / 2));
        }
    }


    public void CacheAnimationLengths()
    {
        _animationClipLengths = new();
        if (_animator == null) _animator = GetComponent<Animator>();
        var controller = _animator.runtimeAnimatorController;

        foreach (var clip in controller.animationClips)
        {
            _animationClipLengths[clip.name] = clip.length;
            //Debug.Log("[Player] Animation Length Cached: " + clip.name + " - " + clip.length);
        }
    }

    public void OnHitLocal(AttackInfo attack)
    {
        if (PlayerFSM.IsDead) return;
        var attacker = attack.Attacker;

        if (Runner.IsServer)
        {
            OnHitStateAuthority(attack);
        }

        //Todo: 맞는 이펙트? 재생
        _impulseSource.GenerateImpulse();
    }

    public void OnHitStateAuthority(AttackInfo attack)
    {
        if (DamagedTimer.ExpiredOrNotRunning(Runner))
        {

            DamagedTimer = TickTimer.CreateFromSeconds(Runner, _damageRecoveryTime);
            if (UnityEngine.Random.Range(0, 1f) < Stat.GetStat(EStatType.EvadeChance))
            {
                Debug.Log("[Player] Evaded damage from " + attack.Attacker);
                return;
            }
            float amount = (attack.MeleeDamage + attack.MagicDamage) * attack.TotalDamageMultiplier;
            float defense = Stat.GetStat(EStatType.Defense);
            float finalDmg = amount * (100 / (100 + defense));
            ParticleManager.Instance.DamageSpawn(finalDmg, transform.position + (Vector3.up * 0.5f), EDamageFloaterType.Damage, true);
            Resource.ConsumeHunger(finalDmg);
            _takedDamage = true;
        }
    }

    private Vector3 _teleportPosition;
    private bool _isTeleporting = false;
    public event Action OnRevive;

    public void Teleport(Vector3 pos)
    {
        if (!HasStateAuthority)
            RPC_Teleport(pos);
        _isTeleporting = true;
        _teleportPosition = pos;

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority,
         HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_Teleport(Vector3 pos, RpcInfo info = default)
    {
        if (!HasStateAuthority) return;
        if (SimpleKCC == null)
        {
            SimpleKCC = GetComponent<SimpleKCC>();
            if (SimpleKCC == null)
            {
                Debug.LogError("[Player] SimpleKCC is not initialized.");
                return;
            }
        }
        _isTeleporting = true;
        _teleportPosition = pos;
    }

    public void Revive()
    {
        
        SimpleKCC.enabled = true;
        Resource.ResetAll();
        OnRevive?.Invoke();

        RPC_ClientRevive();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority,
         HostMode = RpcHostMode.SourceIsHostPlayer)]
    private void RPC_ClientRevive()
    {
        if (_teleportManager == null)
        {
            _teleportManager = FindAnyObjectByType<TeleportManager>();
        }

        ReviveAsync().Forget();
    }

    private TeleportManager _teleportManager;
    public async UniTask ReviveAsync()
    {
        await _teleportManager.ReviveTeleport();
        RPC_ReviveState();
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ReviveState()
    {
        GetComponent<ItemMagnet>().enabled = true;
        PlayerFSM.IsDead = false;

        _nextState = PlayerFSM.StateMachine.GetState<PlayerIdleState>();
    }

    //public void InvokeRevive()
    //{
    //    OnRevive?.Invoke();
    //}

    public void InstantRevive()
    {
        Trait.ResetTraits();
        Stat.ClearAllModifiers();
        Revive();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority,
         HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_RequestInstantRevive(RpcInfo info = default)
    {
        if (!HasStateAuthority) return;

        if (!IsDead) return;
        InstantRevive();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority,
         HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_RequestRevive(RpcInfo info = default)
    {
        if (!HasStateAuthority) return;

        if (!IsDead) return;
        Revive();
    }
}