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
    public Vector3 direction;
    public NetworkButtons buttons;
    public NetworkButtons previousButtons;

    public bool IsHeld(EButtons button) => buttons.IsSet(button);

    public bool WasPressed(EButtons button) =>
        buttons.IsSet(button) && !previousButtons.IsSet(button);

    public bool WasReleased(EButtons button) =>
        !buttons.IsSet(button) && previousButtons.IsSet(button);

}