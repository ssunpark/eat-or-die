using Fusion;
using UnityEngine;

public enum EButtons
{
    Attack = 0,
    Run = 1,
    Jump = 2,
    Interact = 3,
    UseItem = 4,
}

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
    public NetworkButtons buttons;
    public NetworkButtons previousButtons;
    public Vector3 mousePosition;

}