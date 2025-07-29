[System.Serializable]
public class Currency
{
    public ECurrencyType CurrencyType;
    public int Amount;

    public Currency(ECurrencyType currencyType, int amount)
    {
        CurrencyType = currencyType;
        Amount = amount;
    }

}