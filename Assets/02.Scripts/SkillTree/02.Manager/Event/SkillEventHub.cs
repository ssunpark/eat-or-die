using System;
using System.Collections.Generic;

public class SkillEventHub : ISkillEventHub
{
    private readonly Dictionary<ESkillEventType, List<IRuntimeSkill>> _routes = new();
    
    public void Subscribe(IRuntimeSkill node)
    {
        if (!_routes.TryGetValue(node.EventType, out var list))
            _routes[node.EventType] = list = new List<IRuntimeSkill>();

        list.Add(node);
    }

    public void Unsubscribe(IRuntimeSkill node)
    {
        if (_routes.TryGetValue(node.EventType, out var list))
            list.Remove(node);
    }

    public void Publish(ESkillEventType type, SkillContext context, ISkillPayload payload)
    {
        if (!_routes.TryGetValue(type, out var list)) return;

        // 서버 권위에서만 실행하려면 여기서 체크
        // if (!ctx.Authority.IsStateAuthority(...)) return;

        for (int i = 0; i < list.Count; i++)
        {
            list[i].TryExecute(payload, context);
        }
    }
}