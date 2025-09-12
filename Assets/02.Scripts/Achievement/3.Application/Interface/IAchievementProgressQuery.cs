public interface IAchievementProgressQuery {
    public long GetValue(string key); // ex) "kills.total", "currency.wallet"
}