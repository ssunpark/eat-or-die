using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
public class TraitTooltipGUI : MonoBehaviour
{
    [SerializeField] private Vector2 guiPosition = new Vector2(10, 10);
    [SerializeField] private Vector2 boxSize = new Vector2(420, 25);
    [SerializeField] private GUIStyle textStyle;

    private CharacterBase _character;
    private TraitManager _traitManager;
    private List<CharacterTraitData> _traitDataList;

    private void Start()
    {
        _character = GetComponent<CharacterBase>();
        _traitManager = _character.Trait;
        _traitDataList = _traitManager.GetAllTraitData().ToList();

        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                richText = true,
                normal = { textColor = Color.white }
            };
        }
    }

    private void TryLazyInit()
    {
        if (_traitManager != null && _traitDataList != null) return;

        _character ??= GetComponent<CharacterBase>();
        _traitManager ??= _character.Trait;
        _traitDataList = _traitManager.GetAllTraitData().ToList();
    }
#if UNITY_EDITOR
    private void OnGUI()
    {
        TryLazyInit();

        if (_traitManager == null || _traitDataList == null) return;

        Vector2 pos = guiPosition;

        foreach (var data in _traitDataList)
        {
            ETraitType type = data.TraitType;
            int level = _traitManager.GetTraitSnapshot().TryGetValue(type, out var lvl) ? lvl : 0;
            string tooltip = $"<b>[{type}]</b> Lv.{level} - {TraitTooltipGenerator.GenerateTooltip(data)}";

            // 라벨 출력
            GUI.Label(new Rect(pos.x, pos.y, boxSize.x, boxSize.y), tooltip, textStyle);

            // + 버튼
            if (GUI.Button(new Rect(pos.x + boxSize.x + 5, pos.y, 25, boxSize.y), "+"))
            {
                _traitManager.ForceSetLevel(type, level + 1, data);
            }

            // - 버튼
            if (GUI.Button(new Rect(pos.x + boxSize.x + 35, pos.y, 25, boxSize.y), "-"))
            {
                _traitManager.ForceSetLevel(type, Mathf.Max(level - 1, 0), data);
            }

            if (!string.IsNullOrEmpty(data.ActionName) &&
            _character is Player player && player.ExpHandler != null)
            {
                if (GUI.Button(new Rect(pos.x + boxSize.x + 65, pos.y, 45, boxSize.y), "EXP"))
                {
                    player.ExpHandler.GrantExp(data.ActionName);
                }
            }

            pos.y += boxSize.y + 4;
        }
    }
#endif
}
