using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

//수현
public class UI_CookingPanel : MonoBehaviour
{
    public GameObject CookingPanel;
    public GameObject RecipePanel;

    private bool isOpen = false;

    private void Start()
    {
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }

    public void OnClickRecipeButton()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            RecipePanel.GetComponent<UI_RecipePanel>().Open();
        }
        else
        {
            RecipePanel.GetComponent<UI_RecipePanel>().Close();
        }
    }

    public void OpenCookingPanel()
    {
        bool isActive = CookingPanel.activeSelf;
        CookingPanel.SetActive(!isActive);
        // InputReader.playerControllerInputBlocked = true;
    }

    public void OnClickCookingButton()
    {
        NetworkRunner Runner = FindObjectOfType<NetworkRunner>();
        if (Runner == null)
        {
            Debug.Log("NetworkRunner를 찾을 수 없습니다!");
            return;
        }
        // 플레이어 Cooking FSM 호출!
       
        CookingManager.Instance.TryStartCookRPC();
        
        // 요리 결과물 테스트를 위해 추가된 임시 코드입니다.
        //CookingPanelManager.Instance.OnCookingCompleted(true);
        // CookingPanelManager.Instance.ProcessCookingResult(); // 수현 테스트 코드
        CloseTab();
    }

    private void CloseTab()
    {
        isOpen = false;
        CookingPanel.SetActive(false);
        RecipePanel.SetActive(false);
    }
}
