using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    [SerializeField]
    private List<UI_SkillNodeGroup> _skillNodeGroups;
    
    private SkillManager _skillManager;

    public void Bind(Player localPlayer)
    {
        _skillManager = localPlayer.Skill;
        _skillManager.OnDataChanged += Refresh;
        foreach (var group in _skillNodeGroups)
        {
            group.Bind(_skillManager);
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