public interface ISkillHandler
{
    public int SkillId { get; }
    public ESkillEventType EventType { get; }
    public bool TryExecute(ISkillPayload payload, SkillContext context); // 어댑터 진입점
    public void Undo(SkillContext context);
}