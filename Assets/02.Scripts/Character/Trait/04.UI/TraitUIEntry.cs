using System.Collections.Generic;
using System.Linq;
using Ricimi;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TraitUIEntry : MonoBehaviour
{
    [Header("Trait")]
    public ETraitType TraitType;

    [Header("Refs (Statistics-With-Icon)")]
    [SerializeField] private Image _iconImage;                 // Image/Icon
    [SerializeField] private TextMeshProUGUI _traitNameText;   // TraitName
    [SerializeField] private TextMeshProUGUI _tooltipText;     // Tooltip-Basic/TraitTooltip
    [SerializeField] private TextMeshProUGUI _currentLvText;   // Value/TraitCurrentLevel
    [SerializeField] private TextMeshProUGUI _maxLvText;       // Value/TraitMaxLevel
    [SerializeField] private Slider _expSlider;                // ExpSlider

    private TraitManager _traitManager;
    private CharacterTraitData _data;

    public GameObject PopupPrefab;

    protected Canvas _canvas;

    protected void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void Bind(TraitManager traitManager, IEnumerable<CharacterTraitData> allData)
    {
        _traitManager = traitManager;
        _data = allData?.FirstOrDefault(d => d.TraitType == TraitType);

        if (_data == null)
        {
            Debug.LogWarning($"[TraitUIEntry] Trait data not found for {TraitType}");
            return;
        }

        _traitNameText.text = string.IsNullOrEmpty(_data.Name) ? TraitType.ToString() : _data.Name;
        _tooltipText.text = TraitTooltipGenerator.GenerateTooltip(_data);
        _maxLvText.text = _data.MaxLevel.ToString();

        TraitIconLoader.LoadIcon(_data.IconPath, sprite =>
        {
            if (_iconImage) _iconImage.sprite = sprite;
        });

        RefreshLevelAndExp();

        _traitManager.OnTraitLeveledUp += OnTraitLeveledUp;
        _traitManager.OnTraitExpGained += OnTraitExpGained;
    }

    public void Unbind()
    {
        if (_traitManager != null)
        {
            _traitManager.OnTraitLeveledUp -= OnTraitLeveledUp;
            _traitManager.OnTraitExpGained -= OnTraitExpGained;
        }
        _traitManager = null;
        _data = null;
    }

    private void OnDestroy() => Unbind();

    // ---- Event Handlers ----
    private void OnTraitLeveledUp(ETraitType type, int levelDelta)
    {
        if (type != TraitType) return;

        RefreshLevelAndExp();
        _tooltipText.text = TraitTooltipGenerator.GenerateTooltip(_data);
    }

    private void OnTraitExpGained(ETraitType type, int gained)
    {
        if (type != TraitType) return;
        RefreshExpOnly();
    }

    // ---- UI Update ----
    private void RefreshLevelAndExp()
    {
        var trait = _traitManager.GetTrait(TraitType);
        if (trait == null) return;

        _currentLvText.text = trait.Level.ToString();
        _maxLvText.text = trait.MaxLevel.ToString();
        ApplyExpToSlider(trait);
    }

    private void RefreshExpOnly()
    {
        var trait = _traitManager.GetTrait(TraitType);
        if (trait == null) return;
        ApplyExpToSlider(trait);
    }

    private void ApplyExpToSlider(Trait trait)
    {
        float expPerLevel = trait.MaxLevel > 0 ? trait.TotalExpRequired / trait.MaxLevel : 1f;

        _expSlider.minValue = 0f;
        _expSlider.maxValue = 1f;

        if (trait.Level >= trait.MaxLevel)
        {
            _expSlider.value = 1f;
            return;
        }

        float ratio = expPerLevel <= 0f ? 0f : Mathf.Clamp01(trait.CurrentExp / expPerLevel);
        _expSlider.value = ratio;
    }

    public void OnClickOpenSkillTree()
    {
        PopupPrefab.GetComponent<SkillTreePopup>().SetData(_traitManager, _data);
        PopupPrefab.GetComponent<DefaultPopup>().Open();
    }
}
