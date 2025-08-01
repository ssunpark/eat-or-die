using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Fusion.Addons.FSM;
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
    private PlayerController _controller;
    private bool _shouldFinishState;
    private float _cachedWalkSpeed;
    private float _cachedRunSpeed;
    private StatManager _statManager;
    private bool _initialized;

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

    // Animation parameter hashes for persistent state
    private static readonly int _isDeadHash = Animator.StringToHash("IsDead");

    private float _lerpSpeed = 10f;

    #region Unity Lifecycle

    public override void Spawned()
    {
        _anim = GetComponent<Animator>();
    }

    public void TryInitialize()
    {
        TryGetComponent(out _controller);
        if (_controller == null || _controller.Stat == null)
            return;
        _statManager = _controller.Stat;
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

    public void OnActionMoment()
    {
        if (_controller?.FSM?.ActiveState is IAnimationActionNotify notify)
        {
            notify.OnActionMoment();
        }
    }

    public void OnAnimationFinished()
    {
        _shouldFinishState = true;
    }



    #endregion

    #region Animation Trigger Control

    public void PlayTrigger(EAnimTrigger trigger)
    {
        if (_anim != null && _triggerHash.TryGetValue(trigger, out int hash))
        {
            _anim.SetTrigger(hash);
        }
        else if (_anim == null)
        {
            Debug.LogError($"[Animator] Animator is null when trying to play trigger: {trigger}");
        }
        else
        {
            Debug.LogError($"[Animator] Trigger not found: {trigger}");
        }
    }

    /// <summary>
    /// Sets the IsDead parameter for persistent dead state animation
    /// This ensures late-joining players see the correct animation state
    /// </summary>
    public void SetDeadState(bool isDead)
    {
        if (_anim != null)
        {
            _anim.SetBool(_isDeadHash, isDead);
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

        if (_shouldFinishState)
        {
            _shouldFinishState = false;

            if (_controller.FSM.ActiveState is IAnimationActionEndNotify endNotify)
                endNotify.OnAnimationFinished();
        }

        // Ensure dead state animation is synchronized for late-joining players
        SyncDeadStateAnimation();
    }

    /// <summary>
    /// Synchronizes the dead animation state based on the current FSM state
    /// This ensures late-joining players see the correct animation
    /// </summary>
    private void SyncDeadStateAnimation()
    {
        if (_controller?.FSM?.ActiveState != null && _anim != null)
        {
            bool isDead = _controller.FSM.ActiveState.StateId == (int)EPlayerState.Dead;
            bool currentAnimState = _anim.GetBool(_isDeadHash);
            
            // Only update if there's a mismatch to avoid unnecessary animator calls
            if (isDead != currentAnimState)
            {
                _anim.SetBool(_isDeadHash, isDead);
            }
        }
    }

    private void UpdateAnimationSpeed()
    {
        float rawSpeed = _controller.GetComponent<NetworkCharacterController>().Velocity.magnitude;
        float normalized = Mathf.InverseLerp(0f, _cachedRunSpeed, rawSpeed);

        float targetSpeed = 0f;

        if (rawSpeed > 0.1f && rawSpeed < _cachedRunSpeed * 0.9f)
        {
            targetSpeed = 0.5f; // 걷기
        }
        else if (rawSpeed >= _cachedRunSpeed * 0.9f)
        {
            targetSpeed = 1f; // 뛰기
        }
        else
        {
            targetSpeed = 0f; // 정지 상태
        }
        float lerpedSpeed = Mathf.Lerp(_anim.GetFloat("Speed"), targetSpeed, _lerpSpeed * Runner.DeltaTime);
        _anim.SetFloat("Speed", lerpedSpeed);
    }


    #endregion

}
