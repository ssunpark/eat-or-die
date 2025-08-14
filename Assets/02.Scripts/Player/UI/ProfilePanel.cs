using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] private Image _profileImage;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _classText;
    public bool IsBinded { get; private set; } = false;
    public bool BindLocal(PlayerCustomizeHandler customizeHandler)
    {
        if (customizeHandler == null)
        {
            Debug.LogError("PlayerCustomizeHandler is null. Cannot bind to ProfilePanel.");
            return false;
        }
        if(IsBinded)
        {
            Debug.LogWarning("ProfilePanel is already binded to a PlayerCustomizeHandler.");
            return false;
        }
        _nicknameText.text = customizeHandler.Nickname;
        _classText.text = customizeHandler.ClassType.ToString();
        IsBinded = true;
        return true;
    }
}
