using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;
public class Player : CharacterBase, IDamageable
{
    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    [Networked] public TickTimer DamagedTimer { get; set; }
    float _damageRecoveryTime;

    public PlayerFSM PlayerFSM;

    private bool _hasPlayerTrackerRef;

    //private PlayerTracker _playerTrackerRef;


    private Dictionary<string, float> _animationClipLengths;

    private Animator _animator;

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
                    PlayerFSM.StateMachine.ForceActivateState<PlayerHitState>();
                    return;
                }
            }
            else if (PlayerFSM.StateMachine.ActiveState is not PlayerDeadState)
            {
                PlayerFSM.StateMachine.ForceActivateState<PlayerDeadState>();
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
                PlayerFSM.StateMachine.ForceActivateState<PlayerDeadState>();
                return;
            }
        }
        else if (current < max * 0.1f && _prevHunger > current)
        {
            if (PlayerFSM.StateMachine.ActiveState is not PlayerBerserkState)
            {
                PlayerFSM.StateMachine.ForceActivateState<PlayerBerserkState>();
                return;
            }
        }
        else if (_prevHunger < current && current >= max * 0.1f)
        {
            if (PlayerFSM.StateMachine.ActiveState is PlayerBerserkState)
            {
                PlayerFSM.StateMachine.ForceActivateState<PlayerRecoverState>();
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
}