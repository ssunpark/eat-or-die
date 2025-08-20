using System.Collections.Generic;
using Ricimi;
using UnityEngine;
using UnityEngine.UI;

public class UI_ToggleMenu : MonoBehaviour
{
    [Header("각 인덱스는 같은 탭을 가리켜야 합니다")]
    public List<GameObject> TabOnGroup;  // 탭 ON 상태 비주얼
    public List<GameObject> TabOffGroup; // 탭 OFF 상태 비주얼
    public List<GameObject> Content;     // 탭별 보여줄 콘텐츠(씬에 미리 배치)

    private List<Toggle> _toggles = new List<Toggle>();
    private int _currentIndex = -1;

    private void Awake()
    {
        _toggles.Clear();
        foreach (var t in GetComponentsInChildren<Toggle>(true))
            _toggles.Add(t);

        for (int i = 0; i < _toggles.Count; i++)
        {
            int idx = i; // 클로저 캡처 주의
            _toggles[i].onValueChanged.AddListener(v => OnToggleChanged(idx, v));
        }
    }

    private void Start()
    {
        // 처음 켜진 토글 찾기 (없으면 0번 기본)
        int initial = 0;
        for (int i = 0; i < _toggles.Count; i++)
        {
            if (_toggles[i].isOn)
            {
                initial = i;
                break;
            }
        }

        ApplySelection(initial);

        // 모든 토글 상태 정합성 맞추기
        for (int i = 0; i < _toggles.Count; i++)
            _toggles[i].SetIsOnWithoutNotify(i == initial);
    }

    private void OnToggleChanged(int index, bool value)
    {
        if (!value)
            return; // true일 때만 처리
        ApplySelection(index);
    }

    private void ApplySelection(int index)
    {
        if (!IsValid(index))
            return;

        for (int i = 0; i < Content.Count; i++)
        {
            bool on = (i == index);
            SafeSetActive(TabOnGroup, i, on);
            SafeSetActive(TabOffGroup, i, !on);
            SafeSetActive(Content, i, on);
        }

        _currentIndex = index;
    }

    private bool IsValid(int index)
    {
        int n = _toggles.Count;
        return index >= 0 && index < n &&
               index < TabOnGroup.Count &&
               index < TabOffGroup.Count &&
               index < Content.Count;
    }

    private static void SafeSetActive(List<GameObject> list, int index, bool active)
    {
        if (index >= 0 && index < list.Count && list[index] != null)
            list[index].SetActive(active);
    }
}