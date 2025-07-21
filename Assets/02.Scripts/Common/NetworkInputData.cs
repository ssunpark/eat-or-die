using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public bool isAttacking;
    public bool isRunning;
    public bool isJumping;
    public bool isInteracting;
}