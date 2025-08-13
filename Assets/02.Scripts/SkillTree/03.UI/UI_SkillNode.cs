using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillNode : MonoBehaviour
{
    [SerializeField]
    private UI_SkillDescription _skillDescription;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Color _activeColor;
    [SerializeField]
    private Color _inactiveColor;

    private int _id;
    private SkillManager _skillManager;

    public void Bind(SkillManager skillManager, int id)
    {
        _id = id;
        _skillManager = skillManager;
    }

    public void Refresh()
    {
        var manager = _skillManager;
        bool isActive = manager.IsActive(_id);
        _image.color = isActive ? _activeColor : _inactiveColor;

        var name = manager.GetLevelName(_id);
        var currentDescription =
            manager.GetRichTextDescription(_id, manager.GetLevel(_id), _skillDescription.DescriptionPointColor);
        var upgradeDescription = manager.GetLevel(_id) < 5
            ? manager.GetRichTextDescription(_id, manager.GetLevel(_id) + 1, _skillDescription.DescriptionPointColor)
            : String.Empty;
        _skillDescription.Refresh(name, currentDescription, upgradeDescription);
    }
}