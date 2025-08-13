using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillNode : MonoBehaviour
{
    [SerializeField]
    private UI_SkillDescription _skillDescription;

    [SerializeField]
    private List<Image> _lines;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Color _activeColor;
    [SerializeField]
    private Color _inactiveColor;
    
    [SerializeField]
    private Color _lineActiveColor;

    private int _id;
    private SkillManager _skillManager;
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(TryUpgrade);
    }

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

        if (manager.GetLevel(_id) >= 5)
        {
            // 하위 라인 활성화
            foreach (var line in _lines)
            {
                var lineColor = line.color;
                lineColor.a = 1;
                line.color = lineColor;
            }
        }

        var name = manager.GetLevelName(_id);
        var currentDescription =
            manager.GetRichTextDescription(_id, manager.GetLevel(_id), _skillDescription.DescriptionPointColor);
        var upgradeDescription = manager.GetLevel(_id) < 5
            ? manager.GetRichTextDescription(_id, manager.GetLevel(_id) + 1, _skillDescription.DescriptionPointColor)
            : String.Empty;
        _skillDescription.Refresh(name, currentDescription, upgradeDescription);
    }

    public void TryUpgrade()
    {
        if (_skillManager.TryUpgrade(_id))
        {
            Refresh();
        }
    }
}