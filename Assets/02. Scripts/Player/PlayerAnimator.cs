using Fusion;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [Networked] public float Speed { get; set; }
    private InputReader _inputReader;

    private Animator _anim;

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
    //public void SetMoveSpeed(float value)
    //{
    //    _anim.SetFloat("Speed", value);
    //}
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            Vector3 moveDirection = inputData.direction;
            Speed = moveDirection.magnitude;
            _anim.SetFloat("Speed", Speed);

        }
    }

}
