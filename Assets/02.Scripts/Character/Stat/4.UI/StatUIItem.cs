using System;
using TMPro;
using UnityEngine;

public class StatUIItem : MonoBehaviour
{
    public EStatType StatType;

    [SerializeField] private TextMeshProUGUI _statName;
    [SerializeField] private TextMeshProUGUI _statNumber;

    private StatManager _statManager;

    private Action<EStatType, StatModifier> _onAdd;
    private Action<EStatType, StatModifier> _onRemove;

    public void Bind(StatManager statManager)
    {
        _statManager = statManager;

        if (_statName)
            _statName.text = StatNameLocalization.Get(StatType);
        // 최초 값 반영
        Refresh();

        // 이벤트 핸들러 준비 & 구독
        _onAdd = (_, __) => Refresh();
        _onRemove = (_, __) => Refresh();
        _statManager.RegisterModifierCallback(StatType, _onAdd, _onRemove);
    }

    public void Unbind()
    {
        if (_statManager != null)
        {
            if (_onAdd != null || _onRemove != null)
                _statManager.UnregisterModifierCallback(StatType, _onAdd, _onRemove);
        }

        _onAdd = null;
        _onRemove = null;
        _statManager = null;
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Refresh()
    {
        if (_statManager == null) return;

        float value = _statManager.GetStat(StatType);
        if (_statNumber) _statNumber.text = Format(value);
    }

    private string Format(float v)
    {
        switch (StatType)
        {
            case EStatType.CritChance:
            case EStatType.HarvestBonusChance:
            case EStatType.CookBonusChance:
            case EStatType.EvadeChance:
            case EStatType.HungerConsumeReduction:

                return $"{(v * 100f):0.#}%";

            default:
                float r = Mathf.Round(v);
                return Mathf.Approximately(v, r) ? ((int)r).ToString() : v.ToString("0.##");
        }
    }

}
