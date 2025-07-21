using UnityEngine;
using Fusion;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : CharacterBase
{
    [HideInInspector] public PlayerAnimator PlayerAnimatorController;

    private NetworkCharacterController _characterController;
    private bool _isSpawned = false;

    public override void Spawned()
    {
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();

        _isSpawned = true;
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (_isSpawned)
        {
            _characterController.maxSpeed = Stat.GetStat(EStatType.MoveSpeed);
            _characterController.jumpImpulse = Stat.GetStat(EStatType.JumpPower);
            _characterController.acceleration = Stat.GetStat(EStatType.Acceleration);
        }
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
                float baseSpeed = Stat.GetStat(EStatType.MoveSpeed);
                float sprintMultiplier = isRunning
                    ? Stat.GetStat(EStatType.SprintingMultiplier)
                    : 1f;
                float moveSpeed = baseSpeed * sprintMultiplier;

                if (_characterController.maxSpeed != moveSpeed)
                {
                    _characterController.maxSpeed = moveSpeed;
                }
                _characterController.Move(moveDirection);

                Resource.ConsumeSatiety(Time.deltaTime * Stat.GetStat(EStatType.ConsumptionRate));
            }
            else
            {
                _characterController.Move(Vector3.zero);
            }

            if (isJumping && _characterController.Grounded)
            {
                float jumpPower = Stat.GetStat(EStatType.JumpPower);
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
        // 이동 중지 처리
    }

    public bool IsGrounded => _characterController.Grounded;

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_PlayAnimTrigger(EAnimTrigger trigger)
    {
        PlayerAnimatorController.PlayTrigger(trigger);
    }
}
