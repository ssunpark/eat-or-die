using System;
using TMPro;
using UnityEngine;

public class UI_SkillDescription : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _skill;
    [SerializeField]
    private TextMeshProUGUI _description;
    [SerializeField]
    private TextMeshProUGUI _upgradeDescription;
    
    public Color DescriptionPointColor;
    
    public Color UpgradeDescriptionPointColor;

    public void Refresh(string skillText, string descriptionText, string upgradeText)
    {
        _skill.text = skillText;
        
        descriptionText = RichTextUtil.RecolorAll(descriptionText, DescriptionPointColor);
        _description.text = descriptionText;

        if (string.IsNullOrEmpty(upgradeText))
        {
            _upgradeDescription.text = String.Empty;
            return;
        }
        upgradeText = RichTextUtil.RecolorAll(upgradeText, UpgradeDescriptionPointColor);
        _upgradeDescription.text = upgradeText;
    }
}