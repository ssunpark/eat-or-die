using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public bool isAttacking; // 좌클릭
    public bool isRunning;
    public bool isJumping;
    public bool isInteracting; // E
    public bool isUsing; // 우클릭
}