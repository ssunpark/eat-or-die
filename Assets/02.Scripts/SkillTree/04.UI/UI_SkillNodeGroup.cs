using System.Collections.Generic;
using UnityEngine;

public class UI_SkillNodeGroup : MonoBehaviour
{
    [SerializeField]
    private ETraitType _traitType;
    public ETraitType TraitType => _traitType;
    
    [SerializeField]
    private List<UI_SkillNode> _skillNodes;
    
    private SkillManager _skillManager;

    public void Bind(SkillManager skillManager)
    {
        _skillManager = skillManager;
        if (_skillNodes.Count != _skillManager.GetSkills(_traitType).Count)
        {
            Debug.LogWarning("스킬 노드와 대입하려는 스킬 수가 맞지 않습니다.");
        }

        var skills = _skillManager.GetSkills(_traitType);
        for (int i = 0; i < _skillNodes.Count; i++)
        {
            _skillNodes[i].Bind(_skillManager, skills[i].Meta.Id);
        }

        Refresh();
    }

    public void Refresh()
    {
        foreach (var node in _skillNodes)
        {
            node.Refresh();
        }
    }
}