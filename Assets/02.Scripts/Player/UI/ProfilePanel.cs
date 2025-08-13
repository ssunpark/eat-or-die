using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanel : MonoBehaviour
{
    [SerializeField] private Image _profileImage;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _classText;
    public void Bind(PlayerCustomizeHandler customizeHandler)
    {
        if (customizeHandler == null)
        {
            Debug.LogError("PlayerCustomizeHandler is null. Cannot bind to ProfilePanel.");
            return;
        }
        _nicknameText.text = customizeHandler.Nickname;
        _classText.text = customizeHandler.ClassType.ToString();

    }
}
