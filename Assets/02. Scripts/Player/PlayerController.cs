using UnityEngine;
using Fusion;
// 플레이어 이동 담당

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController _characterController;
    public float Speed = 5f;

    private void Awake()
    {
        _characterController = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData))
        {
            Vector3 moveDirection = inputData.direction;
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                _characterController.Move(moveDirection.normalized * Speed * Runner.DeltaTime);
            }
        }
    }
}
