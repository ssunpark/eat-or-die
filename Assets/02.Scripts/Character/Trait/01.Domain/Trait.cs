using UnityEngine;
public class Trait
{
    public int Level { get; private set; }
    public float CurrentExp { get; private set; }
    public int MaxLevel { get; private set; }
    public float TotalExpRequired { get; private set; }

    public Trait()
    {
        Level = 0;
        CurrentExp = 0;
    }

    public void Setup(int maxLevel, float totalExpRequired)
    {
        MaxLevel = maxLevel;
        TotalExpRequired = totalExpRequired;
    }

    public void AddExp(float amount)
    {
        if (Level >= MaxLevel)
            return;

        CurrentExp += amount;

        float expPerLevel = TotalExpRequired / MaxLevel;

        while (CurrentExp >= expPerLevel && Level < MaxLevel)
        {
            CurrentExp -= expPerLevel;
            Level++;
        }

        if (Level >= MaxLevel)
        {
            CurrentExp = 0;
        }
    }

    public void SetLevel(int level)
    {
        Level = Mathf.Clamp(level, 0, MaxLevel);
        CurrentExp = 0;
    }

    
}
