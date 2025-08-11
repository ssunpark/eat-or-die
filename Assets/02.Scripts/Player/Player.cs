using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using Fusion.Addons.SimpleKCC;
using RaycastPro.Detectors;
using UnityEngine;

[RequireComponent(typeof(RangeDetector))]
public class Player : CharacterBase, IAttackable
{
    public TraitExpHandler ExpHandler { get; private set; }
    public List<CharacterTraitData> TraitDataList { get; private set; }
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    [Networked] public TickTimer DamagedTimer { get; set; }


    float _damageRecoveryTime = 0.5f;

    public PlayerFSM PlayerFSM;
    private bool _hasPlayerTrackerRef;

    //private PlayerTracker _playerTrackerRef;
    public PlayerItemHolder ItemHolder { get; private set; }

    private Dictionary<string, float> _animationClipLengths;

    private Animator _animator;
    bool _isReset;
    public SimpleKCC SimpleKCC { get; private set; }

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

    public override void Spawned()
    {
        base.Spawned();
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
        //_hasPlayerTrackerRef = PlayerTracker.GetPlayerTrackerRef(Runner, out _playerTrackerRef);
        Resource.OnHungerChanged += EvaluateCurrentHunger;
        if (HasInputAuthority)
        {
            if (ExpHandler != null && TraitDataList != null)
            {
                LoadTraitsFromStorage();
            }
            else
            {
                StartCoroutine(WaitAndLoadTraits());
            }
                InitializePlayerHUD();
        }

        _animator = GetComponent<Animator>();
        PlayerFSM = GetComponent<PlayerFSM>();
        ItemHolder = GetComponent<PlayerItemHolder>();
        SimpleKCC = GetComponent<SimpleKCC>();
    }
    private IEnumerator WaitAndLoadTraits()
    {
        while (ExpHandler == null || TraitDataList == null)
        {
            yield return null;
        }

        LoadTraitsFromStorage();
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
        }

        Trait.ReapplyAllTraitEffects(TraitDataList);
    }


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }

    public override void FixedUpdateNetwork()
    {
        if(PlayerFSM == null)
        {
            PlayerFSM = GetComponent<PlayerFSM>();
        }
        Stat.UpdateStats(Runner.DeltaTime);

        if (HasInputAuthority)
        {
            if (GetInput<NetworkInputData>(out var input))
            {
                PlayerFSM.SetInput(input);
            }
        }
        else if (HasStateAuthority)
        {
            if(Runner.TryGetInputForPlayer<NetworkInputData>(Object.InputAuthority,out var input))
            {
                PlayerFSM.SetInput(input);
            }
            
        }
        if(HasStateAuthority)
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
    public void RPC_HealOrDamage(float amount)
    {
        if(amount > 0)
        {
            Resource.RestoreHunger(amount);
            // 힐
        }
        else if(amount <0)
        {
            Resource.ConsumeHunger(-amount);
            _takedDamage = true;
            // 데미지
        }
        else
        {
            Debug.Log("미친놈아 0을 왜 호출해");
        }
    }

    public void TryHealOrDamage(float amount)
    {
        RPC_HealOrDamage(amount);
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
            return;
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
                Debug.Log("[Player] Exiting Berserk State due to hunger recovery.");
                _nextState = PlayerFSM.StateMachine.GetState<PlayerRecoverState>();
                _prevHunger = current;
                return;
            }
        }
    }

    public override void Render()
    {
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
        if(_animator==null) _animator = GetComponent<Animator>();
        var controller = _animator.runtimeAnimatorController;

        foreach (var clip in controller.animationClips)
        {
            _animationClipLengths[clip.name] = clip.length;
            //Debug.Log("[Player] Animation Length Cached: " + clip.name + " - " + clip.length);
        }
    }

    
    private void InitializePlayerHUD()
    {
        // 나중에 UIManager를 통해 HUD를 관리할 예정
        GameObject hudObject = GameObject.FindGameObjectWithTag("PlayerHUD");
        if (hudObject != null)
        {
            UI_HUDPlayerHP hudHP = hudObject.GetComponent<UI_HUDPlayerHP>();
            if (hudHP != null)
            {
                hudHP.Initialize(Resource, Stat); // ResourceManager와 StatManager 전달
            }
            else
            {
                Debug.LogError($"HUD 오브젝트 'PlayerHUD'에 UI_HUDPlayerHP 스크립트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"씬에서 태그 'PlayerHUD'를 가진 HUD 오브젝트를 찾을 수 없습니다.");
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
    }

    public void OnHitStateAuthority(AttackInfo attack)
    {
        if (DamagedTimer.ExpiredOrNotRunning(Runner))
        {
            //Todo: 이펙트 처리

            DamagedTimer = TickTimer.CreateFromSeconds(Runner, _damageRecoveryTime);
            float amount = (attack.MeleeDamage + attack.MagicDamage) * attack.TotalDamageMultiplier;
            float defense = Stat.GetStat(EStatType.Defense);
            float finalDmg = amount * (100 / (100 + defense));

            Resource.ConsumeHunger(finalDmg);
            _takedDamage = true;
        }
    }

    public void Revive()
    {
        GetComponent<ItemMagnet>().enabled = true;
        Resource.ResetAll();
        _animator.Play("Idle");
        _nextState = PlayerFSM.StateMachine.GetState<PlayerIdleState>();

        if (_isReset)
        {
            _isReset = false;
            Trait.ResetTraits();
            Stat.ClearAllModifiers();
        }
    }



}