using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
using RaycastPro.Detectors;

[RequireComponent(typeof(RangeDetector))]
public class Player : CharacterBase, IDamageable, IAttackable
{
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    [Networked] public TickTimer DamagedTimer { get; set; }
    float _damageRecoveryTime = 0.5f;

    public PlayerFSM PlayerFSM;
    private bool _hasPlayerTrackerRef;

    //private PlayerTracker _playerTrackerRef;


    private Dictionary<string, float> _animationClipLengths;

    private Animator _animator;
    bool _isReset;
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
            InitializePlayerHUD();
        }

        _animator = GetComponent<Animator>();
        PlayerFSM = GetComponent<PlayerFSM>();

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

        if (_nextState != null)
        {
            PlayerFSM.StateMachine.ForceActivateState(_nextState);
            _nextState = null;
        }

    }


    public void RequestState(EPlayerState state)
    {
        if (PlayerFSM.StateMachine.ActiveState.StateId != (int)state)
            _nextState = PlayerFSM.StateMachine.GetState((int)state);
    }
    public void TakeDamage(float amount, PlayerRef attacker)
    {
        if (DamagedTimer.ExpiredOrNotRunning(Runner))
        {
            //Todo: 이펙트 처리

            float defense = Stat.GetStat(EStatType.Defense);
            float finalDmg = amount * (100 / (100 + defense));

            Resource.ConsumeHunger(finalDmg);
            _takedDamage = true;
        }
    }
    bool _takedDamage = false;
    float _prevHunger;

    APlayerStateBase _nextState = null;

    private void EvaluateCurrentHunger(float current, float max)
    {
        if (_takedDamage)
        {
            _takedDamage = false;
            _prevHunger = current;
            DamagedTimer = TickTimer.CreateFromSeconds(Runner, _damageRecoveryTime);

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
        _prevHunger = current;
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
        else if (current < max * 0.1f && _prevHunger > current)
        {
            if (PlayerFSM.StateMachine.ActiveState is not PlayerBerserkState)
            {
                _nextState = PlayerFSM.StateMachine.GetState<PlayerBerserkState>();
                return;
            }
        }
        else if (_prevHunger < current && current >= max * 0.1f)
        {
            if (PlayerFSM.StateMachine.ActiveState is PlayerBerserkState)
            {
                _nextState = PlayerFSM.StateMachine.GetState<PlayerRecoverState>();
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
            Debug.Log("[Player] Animation Length Cached: " + clip.name + " - " + clip.length);
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

    public void OnHitLocal(AttackInfo attack, NetworkObject attacker)
    {
        RPC_HitByAttack(attack, attacker);
        
        //Todo: 맞는 이펙트? 재생
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_HitByAttack(AttackInfo attack, NetworkObject attacker)
    {
        OnHitStateAuthority(attack, attacker);
    }

    public void OnHitStateAuthority(AttackInfo attack, NetworkObject attacker)
    {
        if (DamagedTimer.ExpiredOrNotRunning(Runner))
        {
            //Todo: 이펙트 처리
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