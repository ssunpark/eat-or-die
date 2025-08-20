using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GenerateInviteCode : MonoBehaviour
{
    [Header("UI Elements")] public Button createCodeButton;
    public TextMeshProUGUI inviteCodeText;

    private void Start()
    {
        createCodeButton.onClick.AddListener(OnCreateCodeButtonClicked);
    }

    public async void OnCreateCodeButtonClicked()
    {
        UI_Notification.Notify("초대 코드가 복사되었습니다.");
        createCodeButton.interactable = false;
        var code = await RoomInfoManager.Instance.GenerateInviteCode();

        if (!string.IsNullOrEmpty(code))
        {
            inviteCodeText.text = code;
        }
        createCodeButton.interactable = true;
        GUIUtility.systemCopyBuffer = inviteCodeText.text;
    }
}