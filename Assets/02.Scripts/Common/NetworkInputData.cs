using Fusion;
using UnityEngine;

enum EButtons
{
    Attack = 0,
    Run = 1,
    Jump = 2,
    Interact = 3,
    UseItem = 4,
}

public struct NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public NetworkButtons buttons;
}