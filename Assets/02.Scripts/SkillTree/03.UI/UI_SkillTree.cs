using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField]
    private List<UI_SkillNodeGroup> _skillNodeGroups;
    
    private void Awake()
    {
        SkillManager.Instance.OnDataChanged += Refresh;
        foreach (var group in _skillNodeGroups)
        {
            group.Bind(SkillManager.Instance.GetSkills(group.TraitType));
        }
    }

    private void Refresh()
    {
        foreach (var group in _skillNodeGroups)
        {
            group.Refresh();
        }
    }
}