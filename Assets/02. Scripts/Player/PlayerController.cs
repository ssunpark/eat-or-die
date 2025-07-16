using UnityEngine;
using Fusion;
// 플레이어 이동 담당

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _characterController;

    [HideInInspector] public PlayerAnimator PlayerAnimatorController;

    [HideInInspector] public PlayerStatManager PlayerStat;
    private Vector3 _direction;
    private bool _isSpawned = false;


    public override void Spawned()
    {
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();

        var installer = GetComponent<PlayerStatInstaller>();
        PlayerStat = installer.StatManager;

        _isSpawned = true;
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (_isSpawned)
        {
            _characterController.maxSpeed = PlayerStat.GetStat(EStatType.MoveSpeed);
            _characterController.jumpImpulse = PlayerStat.GetStat(EStatType.JumpPower);
            _characterController.acceleration = PlayerStat.GetStat(EStatType.Acceleration);
        }
    }

    public void Move(Vector3 direction, float speed)
    {
        if (_characterController.maxSpeed != speed)
        {
            _characterController.maxSpeed = speed;
        }
        _direction = direction;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {

            Vector3 moveDirection = inputData.direction;
            bool isInteracting = inputData.isInteracting;
            bool isJumping = inputData.isJumping;
            bool isRunning = inputData.isRunning;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                float baseSpeed = PlayerStat.GetStat(EStatType.MoveSpeed);
                float sprintMultiplier = isRunning
                    ? PlayerStat.GetStat(EStatType.SprintingMultiplier)
                    : 1f;
                float moveSpeed = baseSpeed * sprintMultiplier;

                if (_characterController.maxSpeed != moveSpeed)
                {
                    _characterController.maxSpeed = moveSpeed;
                }
                _characterController.Move(moveDirection);
            }
            else
            {
                _characterController.Move(Vector3.zero);
            }

            // Handle jumping
            if (isJumping && _characterController.Grounded)
            {
                float jumpPower = PlayerStat.GetStat(EStatType.JumpPower);
                if (_characterController.jumpImpulse != jumpPower)
                {
                    _characterController.jumpImpulse = jumpPower;
                }
                _characterController.Jump();
                Rpc_PlayAnimTrigger(EAnimTrigger.Jump);
            }

        }
        else
        {
            _characterController.Move(Vector3.zero);
        }
    }


    public void Jump(float jumpPower)
    {
        if (_characterController.jumpImpulse != jumpPower)
        {
            _characterController.jumpImpulse = jumpPower;
        }
    }

    public void Stop()
    {
        _direction = Vector3.zero;
    }

    public bool IsGrounded => _characterController.Grounded;

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_PlayAnimTrigger(EAnimTrigger trigger)
    {
        PlayerAnimatorController.PlayTrigger(trigger);
    }
}