using UnityEngine;

public class UI_CraftPanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Craft;
    public GameObject CraftPanel;
    public UI_CraftItemList CraftItemList;
    private bool _isInitalized;


    private void Start()
    {
        CraftPanel.SetActive(false);
        CraftItemList.Init();
    }

    public override void Open()
    {
        base.Open();
        if (!_isInitalized)
        {
            Init();
            _isInitalized = true;
        }
    }

    private void Init()
    {
        CraftItemList.Init();
    }
}