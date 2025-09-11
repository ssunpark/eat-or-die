using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TooltipPanel : MonoBehaviour
{
    public TextMeshProUGUI tooltipText;
    public RectTransform panelRectTransform;
    public float fixedWidth = 450f; // 레시피용 고정 width
    public float minWidth = 100f;   // 최소 width
    public float maxWidth = 300f;   // 최대 width (재료용)

    private void Awake()
    {
        if (panelRectTransform == null)
        {
            panelRectTransform = GetComponent<RectTransform>();
        }
    }

    public void SetText(string content)
    {
        SetText(content, true); // 기본값으로 유동적 width 사용
    }
    
    public void SetText(string content, bool useFlexibleWidth)
    {
        if (tooltipText != null)
        {
            tooltipText.text = content;
        }

        if (panelRectTransform != null)
        {
            if (useFlexibleWidth)
            {
                // 재료 버튼용: 텍스트 길이에 따른 유동적 width
                SetFlexibleWidth();
            }
            else
            {
                // 레시피 버튼용: 고정 width
                SetFixedWidth();
            }
        }
    }

    private void SetFlexibleWidth()
    {
        if (tooltipText != null)
        {
            // 한 줄로 강제 설정
            tooltipText.enableWordWrapping = false;
            tooltipText.overflowMode = TextOverflowModes.Overflow;
            
            // 텍스트 렌더링 강제 업데이트
            Canvas.ForceUpdateCanvases();
            
            // 텍스트의 preferredWidth 기반으로 width 계산
            float preferredWidth = tooltipText.preferredWidth + 40f; // 여백 증가
            float clampedWidth = Mathf.Clamp(preferredWidth, minWidth, maxWidth);
            
            panelRectTransform.sizeDelta = new Vector2(clampedWidth, panelRectTransform.sizeDelta.y);
        }
    }

    private void SetFixedWidth()
    {
        if (tooltipText != null)
        {
            // 레시피 버튼용: 줄바꿈 허용
            tooltipText.enableWordWrapping = true;
            tooltipText.overflowMode = TextOverflowModes.Overflow;
        }
        
        panelRectTransform.sizeDelta = new Vector2(fixedWidth, panelRectTransform.sizeDelta.y);
    }
}