using Fusion;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [Networked] private float Speed { get; set; }

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

    public override void FixedUpdateNetwork()
    {

        if (GetInput(out NetworkInputData inputData))
        {
            Vector3 moveDirection = inputData.direction;
            _anim.SetFloat("Speed", moveDirection.magnitude);

            if(inputData.isAttacking)
            {
                _anim.SetTrigger("Attack");
            }
        }
    }

}
