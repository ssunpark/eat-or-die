using UnityEngine;
using Fusion;
// 플레이어 이동 담당

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _characterController;

    [HideInInspector] public PlayerAnimator PlayerAnimatorController;
    public float Speed = 5f;

    public override void Spawned()
    {
        _characterController = GetComponent<NetworkCharacterController>();
        PlayerAnimatorController = GetComponent<PlayerAnimator>();
    }
    public void Move(Vector3 direction, float speed)
    {
        _characterController.Move(direction.normalized * speed * Runner.DeltaTime);
    }
    
}
