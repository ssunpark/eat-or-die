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
    UseItem
}
public class PlayerAnimator : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnSpeedChanged))]
    public float Speed { get; set; }
    private InputReader _inputReader;
    private Animator _anim;
    private PlayerController _controller;
    private bool _shouldFinishState;

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
        { EAnimTrigger.UseItem, Animator.StringToHash("UseItem") }
    };

    private float _lerpSpeed = 10f;

    #region Unity Lifecycle

    public override void Spawned()
    {
        _controller = GetComponent<PlayerController>();
        _anim = GetComponentInChildren<Animator>();
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
        if (_triggerHash.TryGetValue(trigger, out int hash))
        {
            _anim.SetTrigger(hash);
#if UNITY_EDITOR
            Debug.Log($"[Animator] Triggered: {trigger} (hash: {hash})");
#endif
        }
        else
        {
            Debug.LogError($"[Animator] Trigger not found: {trigger}");
        }
    }

    private void OnSpeedChanged()
    {
        _anim.SetFloat("Speed", Speed);
    }

    #endregion

    #region Network Loop

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority && GetInput(out NetworkInputData inputData))
        {
            float newSpeed = inputData.direction.sqrMagnitude > 0.01f
                ? (_inputReader.IsRunning ? 1f : 0.5f)
                : 0f;

            Rpc_UpdateSpeed(Mathf.Lerp(Speed, newSpeed, _lerpSpeed * Runner.DeltaTime));
        }
        if (_shouldFinishState)
        {
            _shouldFinishState = false;

            if (_controller.FSM.ActiveState is IAnimationActionEndNotify endNotify)
                endNotify.OnAnimationFinished();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_UpdateSpeed(float speed)
    {
        Speed = speed;
    }

    #endregion

}
