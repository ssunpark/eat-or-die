using Fusion;
using UnityEngine;

public class PlayerMove:NetworkBehaviour
{
    private NetworkCharacterController _characterController;
    private StatManager _stat;
    private PlayerController _controller;
    private ResourceManager _resource;
    private Vector3 _dir;

    private float _moveSpeed;
    private float _sprintMultipler;
    private float _accelerationSpeed;
    private float _jumpImpulse;
    public void Initialize(StatManager stat, NetworkCharacterController characterController, PlayerController playerController, ResourceManager resourceManager)
    {
        _characterController = characterController;
        _stat = stat;
        _controller = playerController;
        _resource = resourceManager;

        stat.RegisterModifierCallback(
        EStatType.MoveSpeed,
        (type, mod) => UpdateStatCache(),
        (type, mod) => UpdateStatCache()
    );
        stat.RegisterModifierCallback(
            EStatType.SprintingMultiplier,
            (type, mod) => UpdateStatCache(),
            (type, mod) => UpdateStatCache()
        );
        UpdateStatCache();
    }
    private void UpdateStatCache()
    {
        _moveSpeed = _stat.GetStat(EStatType.MoveSpeed);
        _sprintMultipler = _stat.GetStat(EStatType.SprintingMultiplier);
        _jumpImpulse = _stat.GetStat(EStatType.JumpPower);
        _accelerationSpeed = _stat.GetStat(EStatType.Acceleration);

        _characterController.maxSpeed = _moveSpeed;
        _characterController.jumpImpulse = _jumpImpulse;
        _characterController.acceleration = _accelerationSpeed;
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

        float sprintMultiplier = isRunning
            ? _sprintMultipler
            : 1f;
        float moveSpeed = _moveSpeed * sprintMultiplier;

        if (_characterController.maxSpeed != moveSpeed)
        {
            _characterController.maxSpeed = moveSpeed;
        }
    }

    private void HandleJump(NetworkInputData inputData)
    {
        if (inputData.isJumping && IsGrounded)
        {
            _characterController.Jump();
            _controller.PlayAnimTriggerNetwork(EAnimTrigger.Jump);
        }
    }

    public bool IsGrounded => _characterController.Grounded;

}