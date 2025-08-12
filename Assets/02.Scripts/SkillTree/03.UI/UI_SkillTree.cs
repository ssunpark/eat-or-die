using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField]
    private List<UI_SkillNodeGroup> _skillNodeGroups;
    
    private void Start()
    {
        SkillManager.Instance.OnDataChanged += Refresh;
        foreach (var group in _skillNodeGroups)
        {
            group.Bind(SkillManager.Instance.GetSkills(group.TraitType));
        }

        Refresh();
    }

    private void Refresh()
    {
        foreach (var group in _skillNodeGroups)
        {
            group.Refresh();
        }
    }
}