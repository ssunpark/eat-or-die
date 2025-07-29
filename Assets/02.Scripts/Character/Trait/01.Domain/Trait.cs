public class Trait
{
    public readonly int MaxLevel = 100; // 최대 레벨
    public int Level { get; private set; }
    public int Exp { get; private set; }
    public int ExpToNextLevel => (Level + 1) * 100;

    public Trait()
    {
        Level = 0;
        Exp = 0;
    }

    public void AddExp(int amount)
    {
        Exp += amount;
        while (Exp >= ExpToNextLevel)
        {
            Exp -= ExpToNextLevel;
            Level++;
        }
    }

    public void SetLevel(int level)
    {
        Level = level;
        Exp = 0;
    }
}
