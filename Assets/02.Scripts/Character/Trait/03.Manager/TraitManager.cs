using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TraitManager
{
    public event Action<ETraitType, int> OnTraitLeveledUp;    // (타입, 레벨 증가량)
    public event Action<ETraitType, int> OnTraitExpGained;    // (타입, 획득량)
    public event Action<ETraitType> OnSkillPointChanged; // (타입, 수치)
    public event Action<ETraitType, int> OnTraitLevelSet;

    private Dictionary<ETraitType, int> _skillPoints = new(); // 5레벨 단위 포인트
    private Dictionary<ETraitType, Trait> _traitDict;
    private readonly StatManager _statManager;
    private List<CharacterTraitData> _originalTraitDataList;

    public TraitManager(ITraitDataRepository repo, StatManager statManager)
    {
        _statManager = statManager;

        _traitDict = repo.GetCharacterTraitData().ToDictionary(
            data => data.TraitType,
            data =>
            {
                var trait = new Trait();
                trait.Setup(data.MaxLevel, data.ExpPerLevel);
                trait.SetLevel(0);
                ApplyTraitEffect(data, 0); // 초기화 시 적용
                return trait;
            });

        _originalTraitDataList = repo.GetCharacterTraitData();
    }

    public IEnumerable<CharacterTraitData> GetAllTraitData() => _originalTraitDataList;

    public void AddExp(ETraitType type, int amount, CharacterTraitData traitData)
    {
        if (!_traitDict.TryGetValue(type, out var trait))
        {
            Debug.LogWarning($"[TraitManager] 알 수 없는 특성 타입: {type}");
            return;
        }

        //Debug.Log($"[TraitManager] {type} 특성에 경험치 추가: {amount}");
        int prevLevel = trait.Level;
        if (prevLevel == trait.MaxLevel)
            return;
        trait.AddExp(amount);
        OnTraitExpGained?.Invoke(type, amount);

        if (trait.Level > prevLevel)
        {
            ApplyTraitEffect(traitData, trait.Level);
            OnTraitLeveledUp?.Invoke(type, trait.Level - prevLevel);
            TraitLevelStorage.SetLevel(type, trait.Level);
            int prevPoint = prevLevel / 5;
            int newPoint = trait.Level / 5;
            if (newPoint > prevPoint)
            {
                int gainedPoint = newPoint - prevPoint;
                _skillPoints.TryAdd(type, 0);
                _skillPoints[type] += gainedPoint;
                OnSkillPointChanged?.Invoke(type);
                TraitLevelStorage.SetSkillPoint(type, _skillPoints[type]);
                Debug.Log($"스킬 포인트 획득{type}: {_skillPoints[type]}");
            }

            if (trait.Level == trait.MaxLevel)
            {
                _skillPoints.TryAdd(type, 0);
                _skillPoints[type] += 5;
                OnSkillPointChanged?.Invoke(type);
                TraitLevelStorage.SetSkillPoint(type, _skillPoints[type]);
                Debug.Log($"스킬 포인트 획득{type}: {_skillPoints[type]}");
            }
            //Debug.Log($"[TraitManager] {type} 레벨업: {prevLevel} -> {trait.Level}, 획득 경험치: {amount}");
        }
        //Debug.Log($"[TraitManager] {type} 특성 경험치 저장: {trait.CurrentExp}");

        TraitLevelStorage.SetExperience(type, trait.CurrentExp);
    }

    private void ApplyTraitEffect(CharacterTraitData data, int level)
    {
        float delta = data.ValuePerLevel * level;

        var modifier = new StatModifier(
            value: delta,
            type: data.ModifierType == EStatModifierType.Add
                ? EStatModifierType.Add
                : EStatModifierType.Multiply,
            duration: -1f,
            isBuff: false,
            source: data.TraitType
        );

        _statManager.ApplyModifier(data.StatType, modifier);
    }

    public Dictionary<ETraitType, int> GetTraitSnapshot()
    {
        return _traitDict.ToDictionary(x => x.Key, x => x.Value.Level);
    }

    public void ForceSetLevel(ETraitType type, int level, CharacterTraitData traitData)
    {
        if (!_traitDict.TryGetValue(type, out var trait))
            return;

        int oldLevel = trait.Level;
        trait.SetLevel(level);

        if (level > oldLevel)
        {
            ApplyTraitEffect(traitData, level);
        }
        else if (level < oldLevel)
        {
            // 기존 효과 제거 후 재적용
            _statManager.RemoveModifiersFrom(type);
            ApplyTraitEffect(traitData, level);
        }

        OnTraitLevelSet?.Invoke(type, trait.Level);
    }

    public void ReapplyAllTraitEffects(IEnumerable<CharacterTraitData> allTraitData)
    {
        // 먼저 이전 모디파이어 모두 제거
        foreach (var kvp in _traitDict)
        {
            _statManager.RemoveModifiersFrom(kvp.Key);
        }

        // 현재 레벨 기준으로 다시 적용
        foreach (var data in allTraitData)
        {
            if (_traitDict.TryGetValue(data.TraitType, out var trait))
            {
                ApplyTraitEffect(data, trait.Level);
                OnTraitLevelSet?.Invoke(data.TraitType, trait.Level);
            }
        }
    }

    public void ResetTraits()
    {
        foreach (var kvp in _traitDict)
        {
            var type = kvp.Key;
            kvp.Value.SetLevel(0);
            _statManager.RemoveModifiersFrom(type);
            OnTraitLevelSet?.Invoke(type, 0);

            TraitLevelStorage.SetLevel(type, 0);
            TraitLevelStorage.SetExperience(type, 0f);
            TraitLevelStorage.SetSkillPoint(type, 0);
        }

        _skillPoints.Clear();
        
    }

    public Trait GetTrait(ETraitType type)
    {
        _traitDict.TryGetValue(type, out var trait);
        return trait;
    }

    public void LoadAllSkillPoints(ETraitType type)
    {
        _skillPoints[type] = TraitLevelStorage.GetSkillPoint(type);
    }
    
    public int GetSkillPoints(ETraitType type) => _skillPoints.GetValueOrDefault(type, 0);

    public bool TryUseSkillPoint(ETraitType type)
    {
        if (!_skillPoints.TryGetValue(type, out var sp) || sp <= 0)
        {
            return false;
        }

        _skillPoints[type] -= 1;
        OnSkillPointChanged?.Invoke(type);
        TraitLevelStorage.SetSkillPoint(type, _skillPoints[type]);
        
        return true;
    }
}