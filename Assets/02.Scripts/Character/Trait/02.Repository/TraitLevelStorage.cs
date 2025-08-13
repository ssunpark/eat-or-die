using UnityEngine;

public static class TraitLevelStorage
{
    public static int GetLevel(ETraitType trait)
    {
        return PlayerPrefs.GetInt($"TraitLevel_{trait}", 0);
    }

    public static void SetLevel(ETraitType trait, int level)
    {
        PlayerPrefs.SetInt($"TraitLevel_{trait}", level);
    }

    public static void SetExperience(ETraitType trait, float experience)
    {
        PlayerPrefs.SetFloat($"TraitExperience_{trait}", experience);
    }

    public static float GetExperience(ETraitType trait)
    {
        return PlayerPrefs.GetFloat($"TraitExperience_{trait}", 0);
    }
    
    public static int GetSkillPoint(ETraitType trait)
    {
        return PlayerPrefs.GetInt($"TraitSkillPoint_{trait}", 0);
    }

    public static void SetSkillPoint(ETraitType trait, int skillPoint)
    {
        PlayerPrefs.SetInt($"TraitSkillPoint_{trait}", skillPoint);
    }
}