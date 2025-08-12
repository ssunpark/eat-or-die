using System.Collections.Generic;
using UnityEngine;

public class UI_SkillNodeGroup : MonoBehaviour
{
    [SerializeField]
    private ETraitType _traitType;
    public ETraitType TraitType => _traitType;
    
    [SerializeField]
    private List<UI_SkillNode> _skillNodes;

    public void Bind(List<Skill> skills)
    {
        if (_skillNodes.Count != skills.Count)
        {
            Debug.LogWarning("스킬 노드와 대입하려는 스킬 수가 맞지 않습니다.");
        }

        for (int i = 0; i < _skillNodes.Count; i++)
        {
            _skillNodes[i].Bind(skills[i].Meta.Id);
        }
    }

    public void Refresh()
    {
        foreach (var node in _skillNodes)
        {
            node.Refresh();
        }
    }
}