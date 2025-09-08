/// Host에서 확정된 도메인 이벤트(전투/경제 등)
public class AchievementEvent
{
    public string Key { get; }   // "KillConfirmed", "CurrencyChanged" ...
    public int Amount { get; }   // +1, +100 ...
    public string? Tag { get; }  // "Orc", "Boss" ...

    public AchievementEvent(string key, int amount, string? tag = null)
    {
        Key = key;
        Amount = amount;
        Tag = tag;
    }
}