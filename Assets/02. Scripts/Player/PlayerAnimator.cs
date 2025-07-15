using System.Collections.Generic;
using Fusion;
using UnityEngine;
public enum EAnimTrigger
{
    Attack,
    Farming,
    Cook,
    CookDone,
    Jump,
    GiveFood,
    Die
}
public class PlayerAnimator : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnSpeedChanged))]
    public float Speed { get; set; }
    private InputReader _inputReader;
    private Animator _anim;

    private static readonly Dictionary<EAnimTrigger, int> _triggerHash = new(){
    { EAnimTrigger.Attack, Animator.StringToHash("Attack") },
    { EAnimTrigger.Farming, Animator.StringToHash("Farming") },
    { EAnimTrigger.Cook, Animator.StringToHash("Cook") },
    { EAnimTrigger.CookDone, Animator.StringToHash("CookDone") },
    { EAnimTrigger.Jump, Animator.StringToHash("Jump") },
    { EAnimTrigger.GiveFood, Animator.StringToHash("GiveFood") },
    { EAnimTrigger.Die, Animator.StringToHash("Die") }
    };

    private float _targetSpeed = 0f;
    private float _lerpSpeed = 10f;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _inputReader = FindAnyObjectByType<InputReader>();
        if (_inputReader == null)
        {
            Debug.LogError("InputReader not found in the scene.");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            if (GetInput(out NetworkInputData inputData))
            {
                float newSpeed = inputData.direction.sqrMagnitude > 0.01f
                    ? (_inputReader.IsRunning ? 1f : 0.5f)
                    : 0f;

                Rpc_UpdateSpeed(Mathf.Lerp(Speed, newSpeed, _lerpSpeed * Runner.DeltaTime));
            }
        }

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_UpdateSpeed(float speed)
    {
        Speed = speed;
    }



    public void PlayTrigger(EAnimTrigger trigger)
    {
        if (!_triggerHash.ContainsKey(trigger))
        {
            Debug.LogError($"Trigger {trigger} is not registered in hash dictionary.");
            return;
        }

        int hash = _triggerHash[trigger];
        Debug.Log($"Playing trigger {trigger} with hash {hash}");
        _anim.SetTrigger(hash);
    }
    private void OnSpeedChanged()
    {
        _anim.SetFloat("Speed", Speed);
    }

}
