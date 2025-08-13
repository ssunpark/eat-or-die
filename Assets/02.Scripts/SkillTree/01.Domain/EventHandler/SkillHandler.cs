using System.Collections.Generic;

public class SkillHandler : ISkillHandler
{
    private int _skillId;
    public int SkillId => _skillId;

    private readonly ESkillEventType _eventType;
    public ESkillEventType EventType => _eventType;

    public readonly ISkillEffect Effect;
    public readonly List<ISkillTrigger<ISkillPayload>> Triggers;

    public SkillHandler(int skillId, ESkillEventType type, List<ISkillTrigger<ISkillPayload>> triggers, ISkillEffect effect)
    {
        _skillId = skillId;
        _eventType = type;
        Effect = effect;
        Triggers = triggers;
    }

    // 허브에서 호출하는 비제네릭 진입점
    public bool TryExecute(ISkillPayload payload, SkillContext context)
    {
        // 모든 트리거 AND 검사
        for (int i = 0; i < Triggers.Count; i++)
            if (!Triggers[i].CanTrigger(payload, context))
                return false;

        // 조건 통과 → 효과 1회 실행
        Effect.Execute(payload, context);
        return true;
    }

    public void Undo(SkillContext context)
    {
        Effect.Undo(context);
    }
}