using UnityEngine;
using Fusion;
using UnityEngine.InputSystem.XR;
// 플레이어 이동 담당

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _characterController;

    [HideInInspector] public PlayerAnimator PlayerAnimatorController;
    [HideInInspector] public PlayerStats PlayerStats;
    private Vector3 _direction;

    public void OnEnable()
    {
        if (PlayerAnimatorController == null)
        {
            PlayerAnimatorController = GetComponent<PlayerAnimator>();
        }
        if (PlayerStats == null)
        {
            PlayerStats = GetComponent<PlayerStats>();
        }
    }
    public override void Spawned()
    {
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
        PlayerStats = GetComponent<PlayerStats>();
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

    public void Jump()
    {
        if (_characterController.Grounded)
        {
            _characterController.Jump();
        }
    }

    public void Stop()
    {
        _direction = Vector3.zero;
    }

    public bool IsGrounded => _characterController.Grounded;


}
