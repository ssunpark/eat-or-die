using UnityEngine;
using Fusion;
using UnityEngine.InputSystem.XR;
using System;
// 플레이어 이동 담당

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _characterController;

    [HideInInspector] public PlayerAnimator PlayerAnimatorController;
    [HideInInspector] public PlayerStat PlayerStat;
    private Vector3 _direction;
    private bool _isSpawned = false;
    private bool _isStatLoaded = false;

    public void OnEnable()
    {
        if (PlayerStat == null)
        {
            PlayerStat = GetComponent<PlayerStat>();
            PlayerStat.OnDictionaryLoaded += OnStatLoaded;
        }
    }

    private void OnDisable()
    {
        if (PlayerStat != null)
            PlayerStat.OnDictionaryLoaded -= OnStatLoaded;
    }
    public override void Spawned()
    {
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        PlayerStat = GetComponent<PlayerStat>();

        _isSpawned = true;
        TryInitialize();
    }

    private void OnStatLoaded()
    {
        _isStatLoaded = true;
        TryInitialize();
    }

    private void TryInitialize()
    {
        if (_isSpawned && _isStatLoaded)
        {
            _characterController.maxSpeed = PlayerStat.GetStat(EStatType.MoveSpeed);
            _characterController.jumpImpulse = PlayerStat.GetStat(EStatType.JumpPower);
            _characterController.acceleration = PlayerStat.GetStat(EStatType.Acceleration);
        }
    }

    public void Move(Vector3 direction, float speed)
    {
        if(_characterController.maxSpeed != speed)
        {
            _characterController.maxSpeed = speed;
        }
        _direction = direction;
    }

    public override void FixedUpdateNetwork()
    {
        if(!Object.HasInputAuthority) return;
        
        _characterController.Move(_direction);
    }

    public void Jump(float jumpPower)
    {
        if (_characterController.jumpImpulse != jumpPower)
        {
            _characterController.jumpImpulse = jumpPower;
        }
        if (_characterController.Grounded)
        {
            _characterController.Jump(); 
            Rpc_PlayAnimTrigger(EAnimTrigger.Jump);
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
