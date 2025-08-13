using UnityEngine;

public class PlayerCustomizeHandlerTests : MonoBehaviour
{
    [Header("Auto Find Target In Scene")]
    [SerializeField] private bool _autoFindOnEnable = true;

    [SerializeField] private PlayerCustomizeHandler _target;

    private Rect _windowRect = new Rect(12, 12, 300, 400);

    private void OnEnable()
    {
        if (_autoFindOnEnable && _target == null)
            _target = FindFirst();
    }

    private void OnGUI()
    {
        _windowRect = GUI.Window(987654, _windowRect, DrawWindow, "Customize Test Tool");
    }

    private void DrawWindow(int id)
    {
        GUILayout.BeginVertical("box");

        if (_target == null)
        {
            GUILayout.Label("Target: <none>");
            if (GUILayout.Button("Find PlayerCustomizeHandler"))
                _target = FindFirst();
            GUILayout.EndVertical();
            GUI.DragWindow();
            return;
        }

        GUILayout.Label($"Target: {_target.name}");

        // 상태 표시
        GUILayout.Label($"Helmet:  {(_target.EquipedHelmet ? "ON" : "OFF")}");
        GUILayout.Label($"Chest:   {(_target.EquipedArmor ? "ON" : "OFF")}");
        GUILayout.Label($"Pants:   {(_target.EquipedLeggings ? "ON" : "OFF")}");
        GUILayout.Label($"Boots:   {(_target.EquipedBoots ? "ON" : "OFF")}");

        GUILayout.Space(6);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Helmet ON")) SetEquip(EArmorType.Helmet, true);
        if (GUILayout.Button("Helmet OFF")) SetEquip(EArmorType.Helmet, false);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Chest ON")) SetEquip(EArmorType.Chestplate, true);
        if (GUILayout.Button("Chest OFF")) SetEquip(EArmorType.Chestplate, false);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Pants ON")) SetEquip(EArmorType.Leggings, true);
        if (GUILayout.Button("Pants OFF")) SetEquip(EArmorType.Leggings, false);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Boots ON")) SetEquip(EArmorType.Boots, true);
        if (GUILayout.Button("Boots OFF")) SetEquip(EArmorType.Boots, false);
        GUILayout.EndHorizontal();

        GUILayout.Space(6);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Find Target")) _target = FindFirst();
        if (GUILayout.Button("Refresh View")) { /* 단순 UI 새로고침용 */ }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    private PlayerCustomizeHandler FindFirst()
    {
#if UNITY_2022_3_OR_NEWER
        return FindFirstObjectByType<PlayerCustomizeHandler>(FindObjectsInactive.Include);
#else
        return FindObjectOfType<PlayerCustomizeHandler>(true);
#endif
    }

    private void SetEquip(EArmorType type, bool equip)
    {
        if (_target == null) return;

        // 멀티플레이 권장 경로: InputAuthority → RPC로 요청
        // (로컬 단독 테스트에서도 동일 코드로 동작)
        try
        {
            _target.RPC_EquipOrUnequipSomething(type, equip);
        }
        catch
        {
            // 혹시 네트워크 러너가 없거나 권한 이슈면 로컬 강제 적용(에디터 단독 테스트용)
            switch (type)
            {
                case EArmorType.Helmet: _target.EquipedHelmet = equip; break;
                case EArmorType.Chestplate: _target.EquipedArmor = equip; break;
                case EArmorType.Leggings: _target.EquipedLeggings = equip; break;
                case EArmorType.Boots: _target.EquipedBoots = equip; break;
            }
        }

        // 로컬 즉시 반영
        _target.SetArmor();
    }
}
