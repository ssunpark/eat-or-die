using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class TraitExpHandler
{
    private readonly TraitManager _traitManager;
    private readonly Dictionary<string, CharacterTraitData> _actionToTraitMap;

    public TraitExpHandler(IEnumerable<CharacterTraitData> dataList, TraitManager traitManager)
    {
        _traitManager = traitManager;
        _actionToTraitMap = dataList
            .Where(d => !string.IsNullOrEmpty(d.ActionName))
            .ToDictionary(d => d.ActionName, d => d);
    }

    public void GrantExp(string actionName, int expAmountOverride = -1)
    {
        //Debug.Log($"[TraitExpHandler] GrantExp Called. Action: {actionName}, Override: {expAmountOverride}");
        if (_actionToTraitMap.TryGetValue(actionName, out var trait))
        {
            int expToAdd = expAmountOverride >= 0 ? expAmountOverride : trait.ExpValue;
            //Debug.Log($"[TraitExpHandler] {actionName} 특성에 경험치 추가: {expToAdd}");
            _traitManager.AddExp(trait.TraitType, expToAdd, trait);
        }
        else
        {
            Debug.LogWarning($"[TraitExpHandler] 알 수 없는 액션 이름: {actionName}");
        }
    }
}
