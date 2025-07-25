using System.Collections.Generic;
using System.Linq;

public class TraitManager
{
    private Dictionary<ETraitType, Trait> _traitDict;
    private readonly StatManager _statManager;

    public TraitManager(ITraitDataRepository repo, StatManager statManager)
    {
        _statManager = statManager;

        _traitDict = repo.GetCharacterTraitData().ToDictionary(
            data => data.TraitType,
            data =>
            {
                var trait = new Trait();
                trait.SetLevel(0);
                ApplyTraitEffect(data, 0); // 초기화 시 적용
                return trait;
            });
    }

    public void AddExp(ETraitType type, int amount, CharacterTraitData traitData)
    {
        if (_traitDict.TryGetValue(type, out var trait))
        {
            int oldLevel = trait.Level;
            trait.AddExp(amount);

            if (trait.Level > oldLevel)
                ApplyTraitEffect(traitData, trait.Level - oldLevel);
        }
    }

    private void ApplyTraitEffect(CharacterTraitData data, int levelDiff)
    {
        float delta = data.ValuePerLevel * levelDiff;

        var modifier = new StatModifier(
            value: delta,
            type: data.ModifierType == EStatModifierType.Add
                ? EStatModifierType.Add
                : EStatModifierType.Multiply,
            duration: -1f,
            isBuff: false,
            source: data.TraitType
        );

        _statManager.ApplyModifier(data.AffectedStat, modifier);
    }

    public Dictionary<ETraitType, int> GetTraitSnapshot()
    {
        return _traitDict.ToDictionary(x => x.Key, x => x.Value.Level);
    }

    public void ForceSetLevel(ETraitType type, int level, CharacterTraitData traitData)
    {
        if (!_traitDict.TryGetValue(type, out var trait)) return;

        int oldLevel = trait.Level;
        trait.SetLevel(level);

        if (level > oldLevel)
        {
            ApplyTraitEffect(traitData, level - oldLevel);
        }
        else if (level < oldLevel)
        {
            // 기존 효과 제거 후 재적용
            _statManager.RemoveModifiersFrom(type);
            ApplyTraitEffect(traitData, level);
        }
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
            }
        }
    }
}
