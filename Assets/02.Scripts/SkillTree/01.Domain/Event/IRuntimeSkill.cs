public interface IRuntimeSkill
{
    public int SkillId { get; }
    public ESkillEventType EventType { get; }
    public bool TryExecute(object payload, SkillContext context); // 어댑터 진입점
}