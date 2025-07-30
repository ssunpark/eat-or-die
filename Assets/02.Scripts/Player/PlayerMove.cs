using Fusion;
using UnityEngine;

public class PlayerMove:NetworkBehaviour
{
    private NetworkCharacterController _characterController;
    private StatManager _stat;
    private PlayerController _controller;
    private ResourceManager _resource;
    private Vector3 _dir;
    public void Initialize(StatManager stat, NetworkCharacterController characterController, PlayerController playerController, ResourceManager resourceManager)
    {
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
        if (_controller.MoveFlag)
        {
            return;
        }

        _characterController.Move(_dir);
        if (GetInput(out NetworkInputData inputData))
        {
            HandleJump(inputData);
        }
    }

    public void Move(Vector3 dir, bool isRunning)
    {
        _dir = dir;

        float baseSpeed = _stat.GetStat(EStatType.MoveSpeed);
        float sprintMultiplier = isRunning
            ? _stat.GetStat(EStatType.SprintingMultiplier)
            : 1f;
        float moveSpeed = baseSpeed * sprintMultiplier;

        if (_characterController.maxSpeed != moveSpeed)
        {
            _characterController.maxSpeed = moveSpeed;
        }


        //if (dir.sqrMagnitude > 0.01f)
        //{

        //    float baseSpeed = _stat.GetStat(EStatType.MoveSpeed);
        //    float sprintMultiplier = isRunning
        //        ? _stat.GetStat(EStatType.SprintingMultiplier)
        //        : 1f;
        //    float moveSpeed = baseSpeed * sprintMultiplier;

        //    if (_characterController.maxSpeed != moveSpeed)
        //    {
        //        _characterController.maxSpeed = moveSpeed;
        //    }
        //    //if (moveSpeed > 0f)
        //    //{
        //    //    _resource.ConsumeHunger(_controller.Runner.DeltaTime * _stat.GetStat(EStatType.HungerConsumptionOverTime));
        //    //}


        //    _characterController.Move(dir);
        //}
        //else
        //{
        //    _characterController.Move(Vector3.zero);
        //}
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