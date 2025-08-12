using System.Collections.Generic;
using UnityEngine;

public class SkillManager : BehaviourSingleton<SkillManager>
{
    private const string SKILL_CSV_PATH = "/SkillCSV/Skill.csv";

    private ISkillEventHub _hub;
    private SkillEventFactory _factory;

    // 데이터 전부(레벨 0부터)
    private readonly Dictionary<int, Skill> _skills = new();
    // 현재 활성 핸들러(있을 때만 값 존재)
    private readonly Dictionary<int, ISkillHandler> _handlers = new();

    private void Awake()
    {
        _hub = new SkillEventHub();
        _factory = new SkillEventFactory();

        var list = CSVLoader<SkillRawData>.LoadCSV($"{Application.streamingAssetsPath}{SKILL_CSV_PATH}");
        foreach (var raw in list)
            _skills[raw.Id] = new Skill(raw, 0);
    }

    // 활성화 + 레벨 지정
    public void Active(int id, int level)
    {
        if (!_skills.TryGetValue(id, out var skill))
            return;

        // 기존 구독/핸들러 정리
        if (_handlers.ContainsKey(id))
        {
            _hub.Unsubscribe(_handlers[id]);
            _handlers.Remove(id);
        }

        // 새 핸들러 생성 + 구독
        var handler = _factory.CreateSkillNode(skill.Meta, level);
        _handlers[id] = handler;
        skill.Level = level; // 데이터 갱신

        if (level > 0)
            _hub.Subscribe(handler);
    }

    // 수치만 변하는 경우 인플레이스 갱신(가능하면)
    public void Upgrade(int id, int newLevel)
    {
        if (!_skills.TryGetValue(id, out var skill))
            return;

        Active(id, newLevel);
    }

    public void Inactive(int id)
    {
        if (!_skills.TryGetValue(id, out var skill))
            return;

        // 구독/핸들러 정리
        if (_handlers.ContainsKey(id))
        {
            _hub.Unsubscribe(_handlers[id]);
            _handlers.Remove(id);
        }

        // 데이터만 레벨 0으로
        skill.Level = 0;
    }

    public void Publish<TPayload>(ESkillEventType type, SkillContext ctx, TPayload payload)
        where TPayload : ISkillPayload
        => _hub.Publish(type, ctx, payload);

    public void Publish(ESkillEventType type, SkillContext ctx)
        => _hub.Publish(type, ctx, null);

    // 선택: 조회 헬퍼
    public bool TryGetSkill(int id, out Skill s) => _skills.TryGetValue(id, out s);
    public int GetLevel(int id) => _skills.TryGetValue(id, out var s) ? s.Level : 0;
    public bool IsActive(int id) => GetLevel(id) > 0;
}