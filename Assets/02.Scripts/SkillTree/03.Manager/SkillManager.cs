using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager
{
    private readonly ISkillEventHub _hub;
    private readonly SkillHandlerFactory _factory;
    private readonly SkillRepository _repository;
    private readonly TraitManager _traitManager;

    // 데이터 전부(레벨 0부터)
    private readonly Dictionary<int, Skill> _skills = new();
    // 현재 활성 핸들러(있을 때만 값 존재)
    private readonly Dictionary<int, ISkillHandler> _handlers = new();
    
    public SkillContext Context { get; private set; }

    public event Action OnDataChanged;
    
    private bool _isRestoring;

    public SkillManager(Player player)
    {
        PlayerPrefs.DeleteAll();
        
        _traitManager = player.Trait;
        _hub = new SkillEventHub();
        _factory = new SkillHandlerFactory();
        _repository = new SkillRepository();
        Context = new SkillContext(player);
        
        foreach (var skill in _repository.LoadSkillRawDataList())
        {
            _skills[skill.Meta.Id] = skill;
        }

        SetSkillTree();
        LoadFromDisk();
    }

    private void SetSkillTree()
    {
        foreach (var kvp in _skills)
        {
            var skill = kvp.Value;
            switch (skill.Meta.Position)
            {
                case 1:
                case 2:
                    skill.AddParent(_skills.Values.FirstOrDefault(s => s.Meta.ETraitType == skill.Meta.ETraitType && s.Meta.Position == 0));
                    break;

                case 3:
                    skill.AddParent(_skills.Values.FirstOrDefault(s => s.Meta.ETraitType == skill.Meta.ETraitType && s.Meta.Position == 1));
                    break;

                case 4:
                    skill.AddParent(_skills.Values.FirstOrDefault(s => s.Meta.ETraitType == skill.Meta.ETraitType && s.Meta.Position == 1));
                    skill.AddParent(_skills.Values.FirstOrDefault(s => s.Meta.ETraitType == skill.Meta.ETraitType && s.Meta.Position == 2));
                    break;

                case 5:
                    skill.AddParent(_skills.Values.FirstOrDefault(s => s.Meta.ETraitType == skill.Meta.ETraitType && s.Meta.Position == 3));
                    break;
            }
        }
    }
    
    public List<Skill> GetSkills(ETraitType traitType) 
        => _skills
            .Where(x => x.Value.Meta.ETraitType == traitType)
            .Select(x => x.Value)
            .ToList();

    // 활성화 + 레벨 지정 (테스트)
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
        if (handler == null)
        {
            UI_Notification.Notify("개발중입니다..아마두..?");
            return;
        }
        _handlers[id] = handler;
        
        skill.SetLevel(level); // 데이터 갱신

        if (level > 0)
            _hub.Subscribe(handler);

        Publish(ESkillEventType.OnSkillUpgrade, Context);
        
        OnDataChanged?.Invoke();
    }
    
    private void Upgrade(int id)
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
        int nextLevel = _skills[id].Level + 1;
        
        var handler = _factory.CreateSkillNode(skill.Meta, nextLevel);
        if (handler == null)
        {
            UI_Notification.Notify("개발중입니다..아마두..?");
            return;
        }
        _handlers[id] = handler;
        
        // 내부 데이터 수정
        _skills[id].SetLevel(nextLevel);
        if (!_traitManager.TryUseSkillPoint(_skills[id].Meta.ETraitType))
        {
            Debug.LogWarning("여기서 이 메시지를 보면 뭔가 잘못된거 입니다..(오시현)");
        }

        if (skill.Level > 0)
            _hub.Subscribe(handler);

        Publish(ESkillEventType.OnSkillUpgrade, Context);
        
        NotifyChangedAndAutoSave();
        
        SaveToDisk();
    }

    public bool TryUpgrade(int id)
    {
        // 검사 진행 후 Upgrade에서 실질적인 데이터 변경
        if (!_skills.TryGetValue(id, out var skill))
        {
            return false;
        }

        if (!skill.CheckUpgradeLevel())
        {
            return false;
        }
        
        if (_traitManager.GetSkillPoints(_skills[id].Meta.ETraitType) < 1)
        {
            UI_Notification.Notify("스킬 포인트가 부족합니다.");
            return false;
        }
        
        Upgrade(id);
        return true;
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
        skill.ResetLevel();

        NotifyChangedAndAutoSave();
    }

    public void Publish<TPayload>(ESkillEventType type, SkillContext ctx, TPayload payload)
        where TPayload : ISkillPayload
        => _hub.Publish(type, ctx, payload);

    public void Publish(ESkillEventType type, SkillContext ctx)
        => _hub.Publish(type, ctx, null);
    
    private void NotifyChangedAndAutoSave()
    {
        if (_isRestoring) return; // 로드 중이면 아무 것도 안 함
        OnDataChanged?.Invoke();
        SaveToDisk();
    }
    
    public void SaveToDisk()
    {
        _repository.SaveSkillDataList(_skills.Values);
    }
    
    public void LoadFromDisk()
    {
        var list = _repository.LoadSkillDataList();

        _isRestoring = true;
        
        // 모두 비활성화
        foreach (var id in _skills.Keys.ToList())
            Inactive(id);
        
        // 저장 항목 복구
        foreach (var e in list)
        {
            if (_skills.ContainsKey(e.Id))
            {
                int clamped = Mathf.Clamp(e.Level, 0, Skill.MAX_LEVEL);
                if (clamped > 0)
                    Active(e.Id, clamped); // 핸들러/구독 복원
            }
        }

        _isRestoring = false;

        OnDataChanged?.Invoke();

        SaveToDisk();
    }

    // 조회 헬퍼
    public bool TryGetSkill(int id, out Skill s) => _skills.TryGetValue(id, out s);
    public int GetLevel(int id) => _skills.TryGetValue(id, out var s) ? s.Level : 0;
    public bool IsActive(int id) => GetLevel(id) > 0;
    public string GetName(int id) => _skills.TryGetValue(id, out var s) ? s.Meta.Name : String.Empty;
    public string GetLevelName(int id) => $"LV.{GetLevel(id)} {GetName(id)}";
    private string GetDescription(int id) => _skills.TryGetValue(id, out var s) ? s.Meta.Description : "";
    public string GetRichTextDescription(int id, int level, Color color)
    {
        var text = RichTextUtil.ColorizePlaceholders(GetDescription(id), color);
        return string.Format(text, _skills[id].Meta.NValue * level);
    }
}