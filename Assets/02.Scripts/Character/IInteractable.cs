public interface IInteractable
{
    bool IsImmediate { get; }
    float InteractionDistanceOffset { get;}

    public void Interact();
}
