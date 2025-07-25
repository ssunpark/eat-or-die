using Fusion;
using UnityEngine;

public class PlayerMove:NetworkBehaviour
{
    private NetworkCharacterController _characterController;
    private PlayerStateMachine _fsm;
    private StatManager _stat;
    private PlayerController _controller;
    private ResourceManager _resource;
    
    public void Initialize(StatManager stat, PlayerStateMachine fsm, NetworkCharacterController characterController, PlayerController playerController, ResourceManager resourceManager)
    {
        _fsm = fsm;
        _characterController = characterController;
        _stat = stat;
        _controller = playerController;
        _resource = resourceManager;

        _characterController.maxSpeed = stat.GetStat(EStatType.MoveSpeed);
        _characterController.jumpImpulse = stat.GetStat(EStatType.JumpPower);
        _characterController.acceleration = stat.GetStat(EStatType.Acceleration);
    }

    public override void FixedUpdateNetwork()
    {
        if (_fsm == null)
        {
            return;
        }

        if (GetInput(out NetworkInputData inputData))
        {
            HandleMove(inputData);

            HandleJump(inputData);
        }
    }

    private void HandleMove(NetworkInputData inputData)
    {
        Vector3 moveDirection = inputData.direction;

        if (moveDirection.sqrMagnitude > 0.01f)
        {

            float baseSpeed = _stat.GetStat(EStatType.MoveSpeed);
            float sprintMultiplier = inputData.isRunning
                ? _stat.GetStat(EStatType.SprintingMultiplier)
                : 1f;


            float moveSpeed = _controller.MoveFlag
                ? 0f
                : (baseSpeed * sprintMultiplier);
            if (_characterController.maxSpeed != moveSpeed)
            {
                _characterController.maxSpeed = moveSpeed;
            }
            if (moveSpeed > 0f)
            {
                _resource.ConsumeSatiety(_fsm.Runner.DeltaTime * _stat.GetStat(EStatType.ConsumptionOverTime));
            }

            _characterController.Move(moveDirection);


        }
        else
        {
            _characterController.Move(Vector3.zero);
        }
    }

    private void HandleJump(NetworkInputData inputData)
    {
        if (inputData.isJumping && IsGrounded)
        {
            float jumpPower = _stat.GetStat(EStatType.JumpPower);
            _characterController.jumpImpulse = jumpPower;
            _characterController.Jump();
            if (Object.HasInputAuthority)
            {
                _controller.Rpc_PlayAnimTrigger(EAnimTrigger.Jump);
            }
        }
    }

    public bool IsGrounded => _characterController.Grounded;

}