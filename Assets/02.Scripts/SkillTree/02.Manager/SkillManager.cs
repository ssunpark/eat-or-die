using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : BehaviourSingleton<SkillManager>
{
    private const string SKILL_CSV_PATH = "/SkillCSV/Skill.csv";
    
    private readonly ISkillEventHub _hub;
    private readonly SkillEventFactory _factory;

    // 스킬 기본 정보 캐싱
    private readonly Dictionary<int, SkillRawData> _rawDataCache = new();
    // 현재 액티브 된 스킬 추적
    private readonly Dictionary<int, IRuntimeSkill> _activeSkills = new();

    public SkillManager(ISkillEventHub hub, SkillEventFactory factory)
    {
        _hub = hub;
        _factory = factory;
    }

    private void Awake()
    {
        var list = CSVLoader<SkillRawData>.LoadCSV($"{Application.streamingAssetsPath}{SKILL_CSV_PATH}");
        foreach (var raw in list)
        {
            _rawDataCache[raw.Id] = raw;
        }
    }

    public void Active(int id, int level)
    {
        if (!_rawDataCache.TryGetValue(id, out var raw)) return;

        // 새 노드 생성
        var node = _factory.CreateSkillNode(raw, level);

        // 기존 노드 있으면 해제
        if (_activeSkills.TryGetValue(id, out var old))
            _hub.Unsubscribe(old);

        _activeSkills[id] = node;
        _hub.Subscribe(node);
    }
    
    public void Inactive(int id)
    {
        if (_activeSkills.Remove(id, out var node))
        {
            _hub.Unsubscribe(node);
        }
    }
    
    public void Upgrade(int id, int newLevel) => Active(id, newLevel);
    
    public void Publish<TPayload>(ESkillEventType type, SkillContext ctx, TPayload payload)
        where TPayload : ISkillPayload
    {
        _hub.Publish(type, ctx, payload);
    }
}