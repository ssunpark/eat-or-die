using Fusion;

public struct AchievementEvent : INetworkStruct
{
    public NetworkString<_32> Key; // "KillConfirmed", "CurrencyChanged"
    public int Amount;             // +1, +100 ...
    public NetworkString<_32> Tag; // "Orc", "Boss"...

    public AchievementEvent(string key = "", int amount = 0, string tag = "")
    {
        Key = key;
        Amount = amount;
        Tag = tag;
    }
}