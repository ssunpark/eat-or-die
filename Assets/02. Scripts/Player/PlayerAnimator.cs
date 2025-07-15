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
    [Networked] public float Speed { get; set; }
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
        if (GetInput(out NetworkInputData inputData))
        {
            Vector3 moveDirection = inputData.direction;

            // 목표 속도 설정
            if (moveDirection.magnitude > 0f)
            {
                _targetSpeed = _inputReader.IsRunning ? 1f : 0.5f;
            }
            else
            {
                _targetSpeed = 0f;
            }

            // 보간 적용
            Speed = Mathf.Lerp(Speed, _targetSpeed, _lerpSpeed * Runner.DeltaTime);
            _anim.SetFloat("Speed", Speed);

        }
    }



    public void PlayTrigger(EAnimTrigger trigger)
    {
        _anim.SetTrigger(_triggerHash[trigger]);
    }


}
