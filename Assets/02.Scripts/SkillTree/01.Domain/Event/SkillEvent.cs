using System.Collections.Generic;

public class SkillEvent<TPayload> : IRuntimeSkill
    where TPayload : ISkillPayload
{
    private int _skillId;
    public int SkillId => _skillId;

    private readonly ESkillEventType _eventType;
    public ESkillEventType EventType => _eventType;

    public readonly ISkillEffect<TPayload> Effect;
    public readonly List<ISkillTrigger<TPayload>> Triggers = new();

    public SkillEvent(int skillId, ESkillEventType type, ISkillEffect<TPayload> effect)
    {
        _skillId = skillId;
        _eventType = type;
        Effect = effect;
    }

    // 허브에서 호출하는 비제네릭 진입점
    public bool TryExecute(object payload, SkillContext context)
    {
        if (payload is not TPayload p)
            return false;

        // 모든 트리거 AND 검사
        for (int i = 0; i < Triggers.Count; i++)
            if (!Triggers[i].CanTrigger(p, context))
                return false;

        // 조건 통과 → 효과 1회 실행
        Effect.Execute(p, context);
        return true;
    }
}