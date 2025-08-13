using System;
using UnityEngine;

[Serializable]
public class SkillDTO
{
    [SerializeField] private int id;
    [SerializeField] private int level;

    public int Id => id;
    public int Level => level;

    public SkillDTO(int id, int level)
    {
        this.id = id;
        this.level = level;
    }
}