using TMPro;
using UnityEngine;

public class SkillTreePopup : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _traitNameText;
    [SerializeField] private TextMeshProUGUI _skillPointText;


    private TraitManager _traitManager;
    private CharacterTraitData _data;

    public void SetData(TraitManager traitManager, CharacterTraitData data)
    {
        _traitManager = traitManager;
        _data = data;

        if (_traitNameText) _traitNameText.text = data.Name;

        if (_descriptionText)
        {
            var tooltip = TraitTooltipGenerator.GenerateTooltip(data);
            _descriptionText.text = tooltip;
        }

        // 첫 표시
        RefreshLevel();

        _traitManager.OnTraitLeveledUp += OnTraitLeveledUp;
    }

    private void OnTraitLeveledUp(ETraitType type, int diff)
    {
        if (_data != null && type == _data.TraitType)
            RefreshLevel();
    }

    private void RefreshLevel()
    {
        var trait = _traitManager.GetTrait(_data.TraitType);
        int curLv = trait?.Level ?? 0;
        if (_levelText) _levelText.text = curLv.ToString();
    }

    private void OnDisable()
    {
        if (_traitManager != null)
            _traitManager.OnTraitLeveledUp -= OnTraitLeveledUp;
    }
}
