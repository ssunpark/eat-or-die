using UnityEngine;

public class UI_ReviveButton : MonoBehaviour
{
    public void OnClickReviveButton()
    {
        ReviveShopManager.Instance?.TryRevive();
    }
        
}