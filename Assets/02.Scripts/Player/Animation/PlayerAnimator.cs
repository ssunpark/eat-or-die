using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;
using DarkTonic.MasterAudio;
public enum EAnimTrigger
{
    Attack,
    Interact,
    Cook,
    CookDone,
    Jump,
    GiveFood,
    Die,
    Hit,
    Berserk,
    UseItem,
    Recover
}
public class PlayerAnimator : NetworkBehaviour
{
    private InputReader _inputReader;
    private Animator _anim;
    private Player _player;
    private PlayerFSM _fsm;
    private float _cachedWalkSpeed;
    private float _cachedRunSpeed;
    private StatManager _statManager;
    private bool _initialized;
    private bool _isMoving;
    private MasterAudio _masterAudio;

    string _footstepSound = "Foot_";


    private static readonly Dictionary<EAnimTrigger, int> _triggerHash = new()
    {
        { EAnimTrigger.Attack, Animator.StringToHash("Attack") },
        { EAnimTrigger.Interact, Animator.StringToHash("Interact") },
        { EAnimTrigger.Cook, Animator.StringToHash("Cook") },
        { EAnimTrigger.CookDone, Animator.StringToHash("CookDone") },
        { EAnimTrigger.Jump, Animator.StringToHash("Jump") },
        { EAnimTrigger.GiveFood, Animator.StringToHash("GiveFood") },
        { EAnimTrigger.Die, Animator.StringToHash("Die") },
        { EAnimTrigger.Hit, Animator.StringToHash("Hit") },
        { EAnimTrigger.Berserk, Animator.StringToHash("Berserk") },
        { EAnimTrigger.UseItem, Animator.StringToHash("UseItem") },
        { EAnimTrigger.Recover, Animator.StringToHash("Recover") }
    };

    private float _lerpSpeed = 10f;

    #region Unity Lifecycle

    public override void Spawned()
    {
        _anim = GetComponent<Animator>();
        _fsm = GetComponent<PlayerFSM>();
    }

    public void TryInitialize()
    {
        _anim = GetComponent<Animator>();
        _fsm = GetComponent<PlayerFSM>();
        TryGetComponent(out _player);
        if (_player == null || _player.Stat == null)
            return;
        _statManager = _player.Stat;
        _statManager.RegisterModifierCallback(
        EStatType.MoveSpeed,
        (type, mod) => UpdateStatCache(),
        (type, mod) => UpdateStatCache()
    );
        _statManager.RegisterModifierCallback(
            EStatType.SprintingMultiplier,
            (type, mod) => UpdateStatCache(),
            (type, mod) => UpdateStatCache()
        );
        UpdateStatCache();
        _initialized = true;
    }

    private void UpdateStatCache()
    {
        _cachedWalkSpeed = _statManager.GetStat(EStatType.MoveSpeed);
        float sprintMultiplier = _statManager.GetStat(EStatType.SprintingMultiplier);
        _cachedRunSpeed = _cachedWalkSpeed * sprintMultiplier;
    }

    private void OnEnable()
    {
        _inputReader = FindAnyObjectByType<InputReader>();
        if (_inputReader == null)
        {
            Debug.LogError("InputReader not found in scene.");
        }
    }

    #endregion
    #region Animation Events (From Clip)

    private void TryInitializeController()
    {
        if (_player == null)
            _player = GetComponent<Player>();
    }
    public void OnActionMoment()
    {
        if (_fsm?.StateMachine?.ActiveState is IAnimationActionNotify notify)
        {
            notify.OnActionMoment();
        }
    }

    public void OnFootStep()
    {
        if (StageManager.Instance == null) return;

        string postfix = StageManager.Instance.CurrentStage switch
        {
            0 => "Leaf",
            1 => "Leaf",
            2 => "Cave",
            3 => "Cave",
            _ => "Snow"
        };

        MasterAudio.PlaySound3DAtTransform(_footstepSound + postfix, transform);
    }

    public void OnAnimationFinished()
    {
        TryInitializeController();
    }




    #endregion

    #region Animation Trigger Control

    public void PlayTrigger(EAnimTrigger trigger)
    {
        if (_triggerHash.TryGetValue(trigger, out int hash))
        {
            _anim.SetTrigger(hash);
        }
        else
        {
            Debug.LogError($"[Animator] Trigger not found: {trigger}");
        }
    }
    

    #endregion

    #region Network Loop

    public override void FixedUpdateNetwork()
    {
        if (!_initialized)
        {
            TryInitialize();
            if (!_initialized)
                return; // 아직 준비 안 됨
        }
        UpdateAnimationSpeed();
    }
    public void SetSpeedParameter(float rawSpeed)
    {
        if (_anim == null) return;
        if (_isMoving == false)
        {
            _targetSpeed = 0f;
            return;
        }
        float normalized = Mathf.InverseLerp(0f, _cachedRunSpeed, rawSpeed);
        if (rawSpeed > 0.1f && rawSpeed < _cachedRunSpeed * 0.9f)
        {
            _targetSpeed = 0.5f; // 걷기
        }
        else if (rawSpeed >= _cachedRunSpeed * 0.9f)
        {
            _targetSpeed = 1f; // 뛰기
        }
        else
        {
            _targetSpeed = 0f; // 정지 상태
        }
    }
    float _targetSpeed;
    private void UpdateAnimationSpeed()
    {
        float current = _anim.GetFloat("Speed");
        float lerpedSpeed = Mathf.Lerp(current, _targetSpeed, _lerpSpeed * Runner.DeltaTime);
        _anim.SetFloat("Speed", lerpedSpeed);
    }


    #endregion

}
