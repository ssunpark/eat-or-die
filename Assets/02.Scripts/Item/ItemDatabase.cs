public static class ItemDatabase
{
    public static bool TryGetWeaponType(int itemID, out EWeaponType type)
    {
        // 나중엔 CSV에서 가지고 오기
        switch (itemID)
        {
            //임시!!!!!
            case 500001:
            case 600001:
            case 600004:
                type = EWeaponType.Sword; return true;
            case 600002:
            case 600005:
                type = EWeaponType.Axe; return true;
            case 600003:
            case 600006:
                type = EWeaponType.Staff; return true;
            default:
                type = default;
                return false;
        }
    }

    public static bool TryGetUseAction(int itemID, out EUseAction action)
    {
        switch (itemID)
        {
            case 500001: action = EUseAction.Plow; return true;
            case 500002: action = EUseAction.Water; return true;
            default: action = default; return false;
        }
    }
}
