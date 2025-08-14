public interface IItemPayload : ISkillPayload
{
    public int ItemId { get; }
    public int ItemQuantity { get; }
}