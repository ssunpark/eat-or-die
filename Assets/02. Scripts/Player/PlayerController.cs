using UnityEngine;
using Fusion;
// 플레이어 이동 담당

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _characterController;

    [HideInInspector] public PlayerAnimator PlayerAnimatorController;
    [HideInInspector] public PlayerStats PlayerStats;
    public float Speed = 5f;

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
            Debug.Log($" {_characterController.maxSpeed}");
        }
        _characterController.Move(direction);
    }
    
}
