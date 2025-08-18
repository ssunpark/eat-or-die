using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GenerateInviteCode : MonoBehaviour
{
    [Header("UI Elements")] public Button createCodeButton; // 코드 생성 버튼
    public Button copyCodeButton; // 코드 복사 버튼
    public TextMeshProUGUI inviteCodeText; // 코드를 표시할 텍스트 (TextMeshPro 기준)

    private void Start()
    {
        // 버튼에 클릭 이벤트 연결
        createCodeButton.onClick.AddListener(OnCreateCodeButtonClicked);
        copyCodeButton.onClick.AddListener(OnCopyCodeButtonClicked);

        // 초기에는 복사 버튼을 비활성화
        copyCodeButton.gameObject.SetActive(false);
        inviteCodeText.text = "초대 버튼을 눌러 코드를 생성하세요.";
    }

    private async void OnCreateCodeButtonClicked()
    {
        // 버튼 비활성화로 중복 생성 방지
        createCodeButton.interactable = false;
        inviteCodeText.text = "코드를 생성하는 중...";

        // RoomInfoManager의 메서드 호출
        var code = await RoomInfoManager.Instance.GenerateInviteCode();

        if (!string.IsNullOrEmpty(code))
        {
            // 성공 시: 코드 표시 및 복사 버튼 활성화
            inviteCodeText.text = code;
            copyCodeButton.gameObject.SetActive(true);
        }
        else
        {
            // 실패 시: 에러 메시지 표시
            inviteCodeText.text = "코드 생성에 실패했습니다.";
        }

        // 버튼 다시 활성화
        createCodeButton.interactable = true;
    }

    private void OnCopyCodeButtonClicked()
    {
        // 클립보드에 현재 텍스트 복사
        GUIUtility.systemCopyBuffer = inviteCodeText.text;
        Debug.Log($"초대 코드 '{inviteCodeText.text}'가 클립보드에 복사되었습니다.");

        // (선택 사항) 사용자에게 "복사 완료!" 같은 피드백을 줄 수 있습니다.
    }
}