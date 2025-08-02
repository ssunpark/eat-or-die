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

    public static NetworkInputData GetLocalInput(Player player, NetworkRunner runner)
    {
        NetworkInputData input = new NetworkInputData();

        // 
        //if( UI (요리, 제작대 등) 열려있을 경우)
        //     return input;

        input.direction = InputReader.Instance.MoveInput;

        input.buttons.Set(EButtons.Attack, InputReader.Instance.IsAttackDown);
        input.buttons.Set(EButtons.Interact, InputReader.Instance.IsInteractDown);
        input.buttons.Set(EButtons.UseItem, InputReader.Instance.IsUseItemDown);
        input.buttons.Set(EButtons.Run, InputReader.Instance.IsSprintDown);

        return input;
    }
}