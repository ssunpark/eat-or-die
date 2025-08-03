using Fusion;
using UnityEngine;

public class PlayerMove:NetworkBehaviour
{
    private NetworkCharacterController _ncc;
    private StatManager _stat;
    private PlayerFSM _controller;
    private ResourceManager _resource;
    private Vector3 _dir;

    private float _moveSpeed;
    private float _sprintMultipler;
    private float _accelerationSpeed;
    private float _jumpImpulse;
    public void Initialize(StatManager stat, NetworkCharacterController characterController, PlayerFSM playerController, ResourceManager resourceManager)
    {
        _ncc = characterController;
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

        _ncc.maxSpeed = _moveSpeed;
        _ncc.jumpImpulse = _jumpImpulse;
        _ncc.acceleration = _accelerationSpeed;
    }
    

    public void Move(Vector3 dir, bool isRunning)
    {
        _dir = dir;
        if (_dir == Vector3.zero)
        {
            _ncc.maxSpeed = 0f;
            return;
        }
        float sprintMultiplier = isRunning
            ? _sprintMultipler
            : 1f;
        float moveSpeed = _moveSpeed * sprintMultiplier;

        

        if (_ncc.maxSpeed != moveSpeed)
        {
            _ncc.maxSpeed = moveSpeed;
        }
    }

    private void HandleJump(NetworkInputData inputData)
    {
        if (inputData.buttons.IsSet(EButtons.Jump) && IsGrounded)
        {
            _ncc.Jump();
        }
    }

    public bool IsGrounded => _ncc.Grounded;

}