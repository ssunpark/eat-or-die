public interface IInteractable
{
    bool IsImmediate { get; }
    float InteractionDistanceOffset { get;}

    Player InteractingPlayer { get; }

    public void Interact();

    public void Interact(Player from);
}
