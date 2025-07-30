using UnityEngine;

public class UI_CraftingTablePanel : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.Craft;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            InputReader.playerControllerInputBlocked = false;
        }
    }
}