public interface IInteractable
{
    EInteractionType Type { get; }
    void Interact(PlayerController player);
}