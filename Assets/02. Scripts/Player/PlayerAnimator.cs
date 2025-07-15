using Fusion;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [Networked] public float Speed { get; set; }
    private InputReader _inputReader;
    private Animator _anim;

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
}
