using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AchievementViewerWindow : EditorWindow
{
    [MenuItem("Tools/AchievementsViewer")]
    public static void Open()
    {
        var win = GetWindow<AchievementViewerWindow>("Achievement Viewer");
        win.Show();
    }

    private int _playerId = 0;               // 로컬 모드면 보통 0
    private string _search = "";
    private bool _showUnlocked = true;
    private bool _showLocked = true;
    private string _categoryFilter = "All";
    private Vector2 _scroll;
    private List<AchievementDto> _cache = new();
    private string[] _categories = new[] { "All" };

    // Metric Tester
    private string _metricKey = "currency.wallet";
    private long _metricValue = 0;

    // ▶ Event Tester (HandleEventLocal 호출용)
    private string _eventKey = "KillConfirmed"; // CSV의 CriteriaKey와 매칭되는 이벤트 이름
    private int _eventDelta = 1;               // 보통 1
    private string _eventTag = "";              // 필요 없으면 공란 (예: enemyType 등 태그)

    private bool Ready =>
        Application.isPlaying &&
        AchievementManager.Instance != null;

    private void OnEnable()
    {
        titleContent = new GUIContent("Achievement Viewer");
        TryRefresh();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }
    private void OnPlayModeChanged(PlayModeStateChange s)
    {
        TryRefresh();
        Repaint();
    }

    private void OnGUI()
    {
        DrawHeader();

        if (!Ready)
        {
            EditorGUILayout.HelpBox(
                "플레이 모드가 아니거나 AchievementManager가 준비되지 않았습니다.\n" +
                "▶ Play를 눌러 실행 중에 열어주세요.",
                MessageType.Info);
            return;
        }

        DrawToolbar();
        DrawMetricTester();   // CounterReach 계열 실험
        DrawEventTester();    // ▶ OneShot/Event 계열 실험 (HandleEventLocal)
        EditorGUILayout.Space(6);
        DrawListTable();
    }

    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUILayout.Label("Achievement Viewer", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Refresh", EditorGUIUtility.IconContent("Refresh").image),
                EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                TryRefresh();
            }

            if (GUILayout.Button(new GUIContent("Dump", "현재 목록을 콘솔에 덤프"),
                EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                DumpToConsole();
            }
        }
    }

    private void DrawToolbar()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _playerId = EditorGUILayout.IntField("Player Id", _playerId, GUILayout.MaxWidth(250));

                GUILayout.Space(10);
                _search = EditorGUILayout.TextField(new GUIContent("Search", "제목/설명에서 검색"),
                    _search, GUILayout.MinWidth(150));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Re-evaluate All", GUILayout.Width(140)))
                {
                    AchievementManager.Instance.ReevaluateAllLocal(_playerId);
                    TryRefresh();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                _showUnlocked = EditorGUILayout.ToggleLeft("Unlocked", _showUnlocked, GUILayout.Width(90));
                _showLocked   = EditorGUILayout.ToggleLeft("Locked",   _showLocked,   GUILayout.Width(80));

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Category", GUILayout.Width(60));
                int idx = Mathf.Max(0, Array.IndexOf(_categories, _categoryFilter));
                int newIdx = EditorGUILayout.Popup(idx, _categories, GUILayout.Width(150));
                if (newIdx != idx) _categoryFilter = _categories[newIdx];

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear Filters", GUILayout.Width(110)))
                {
                    _search = "";
                    _showUnlocked = true;
                    _showLocked = true;
                    _categoryFilter = "All";
                }
            }
        }
    }

    private void DrawMetricTester()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Metric Tester (CounterReach)", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                _metricKey = EditorGUILayout.TextField("Key", _metricKey);
                _metricValue = EditorGUILayout.LongField("Value", _metricValue, GUILayout.MaxWidth(200));

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Set + Re-evaluate", GUILayout.Width(160)))
                {
                    AchievementManager.Instance.SetMetricLocal(_metricKey, _metricValue);
                    AchievementManager.Instance.ReevaluateAllLocal(_playerId);
                    TryRefresh();
                }
            }
            EditorGUILayout.HelpBox(
                "예) currency.wallet=1000 으로 세팅 후 Re-evaluate 하면, 100/200/1000 업적이 한 번에 언락되는지 확인.",
                MessageType.None);
        }
    }

    // ▶ 신규 추가: Event Tester (HandleEventLocal)
    private void DrawEventTester()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Event Tester (HandleEventLocal)", EditorStyles.boldLabel);

            _eventKey = EditorGUILayout.TextField(new GUIContent("Event Key", "CSV의 CriteriaKey(OneShotEvent / 이벤트 트리거 이름)"),
                                                  _eventKey);
            using (new EditorGUILayout.HorizontalScope())
            {
                _eventDelta = EditorGUILayout.IntField(new GUIContent("Delta", "이벤트 값(보통 1)"), _eventDelta, GUILayout.MaxWidth(220));
                _eventTag   = EditorGUILayout.TextField(new GUIContent("Tag", "선택: 적 타입 등"), _eventTag);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Send Event", GUILayout.Width(140)))
                {
                    // AchievementEvent(string key, int delta, int playerId, string tag)
                    var e = new AchievementEvent(_eventKey, _eventDelta, _eventTag);
                    AchievementManager.Instance.HandleEventLocal(e);
                    TryRefresh();
                }
            }

            EditorGUILayout.Space(2);

            // 빠른 프리셋 버튼들 (원하면 삭제 가능)
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("FirstJoin", GUILayout.Width(100)))
                {
                    var e = new AchievementEvent("FirstJoin", 1, "");
                    AchievementManager.Instance.HandleEventLocal(e);
                    TryRefresh();
                }
                if (GUILayout.Button("KillConfirmed +1 (Orc)", GUILayout.Width(180)))
                {
                    var e = new AchievementEvent("KillConfirmed", 1, "Orc");
                    AchievementManager.Instance.HandleEventLocal(e);
                    TryRefresh();
                }
                if (GUILayout.Button("BossKilled", GUILayout.Width(120)))
                {
                    var e = new AchievementEvent("BossKilled", 1, "");
                    AchievementManager.Instance.HandleEventLocal(e);
                    TryRefresh();
                }
                GUILayout.FlexibleSpace();
            }
        }
    }

    private void DrawListTable()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            HeaderCell("ID", 60);
            HeaderCell("Title", 200);
            HeaderCell("Category", 100);
            HeaderCell("Progress", 160);
            HeaderCell("Unlocked", 80);
            HeaderCell("UnlockedAt", 180);
        }
        EditorGUILayout.Space(2);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        foreach (var dto in Filtered(_cache))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                Cell(dto.Id.ToString(), 60);
                Cell(dto.Title, 200, wrap:false, bold: dto.IsUnlocked);
                Cell(dto.Category, 100);

                var prog = dto.Target > 0 ? $"{dto.Current:0}/{dto.Target:0}" : "-";
                Cell(prog, 160);

                Cell(dto.IsUnlocked ? "Yes" : "No", 80);

                var ts = dto.UnlockedAtUtc.HasValue
                    ? dto.UnlockedAtUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                    : "-";
                Cell(ts, 180);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(62);
                var style = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
                EditorGUILayout.LabelField(dto.Description, style);
            }
            EditorGUILayout.Space(6);
        }
        EditorGUILayout.EndScrollView();
    }

    private IEnumerable<AchievementDto> Filtered(IEnumerable<AchievementDto> src)
    {
        IEnumerable<AchievementDto> q = src;

        if (!string.IsNullOrEmpty(_search))
        {
            var s = _search.ToLowerInvariant();
            q = q.Where(x =>
                (x.Title ?? "").ToLowerInvariant().Contains(s) ||
                (x.Description ?? "").ToLowerInvariant().Contains(s));
        }

        if (!(_showUnlocked && _showLocked))
        {
            if (_showUnlocked && !_showLocked) q = q.Where(x => x.IsUnlocked);
            if (_showLocked && !_showUnlocked) q = q.Where(x => !x.IsUnlocked);
        }

        if (!string.IsNullOrEmpty(_categoryFilter) && _categoryFilter != "All")
            q = q.Where(x => x.Category == _categoryFilter);

        return q;
    }

    private void TryRefresh()
    {
        _cache.Clear();
        if (!Ready) return;

        try
        {
            var list = AchievementManager.Instance.GetAchievementDTOList();
            _cache = list?.ToList() ?? new List<AchievementDto>();

            var cats = _cache.Select(x => x.Category).Where(c => !string.IsNullOrEmpty(c))
                             .Distinct().OrderBy(c => c).ToList();
            cats.Insert(0, "All");
            _categories = cats.ToArray();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AchievementViewer] Refresh failed: {ex.Message}");
        }
    }

    private void DumpToConsole()
    {
        if (!Ready) { Debug.LogWarning("[AchievementViewer] Not ready."); return; }
        foreach (var a in Filtered(_cache))
        {
            Debug.Log($"[Ach {a.Id}] {(a.IsUnlocked ? "Unlocked" : "Locked")}  " +
                      $"({a.Current}/{a.Target})  [{a.Category}]  {a.Title}");
        }
    }

    // GUI helpers
    private void HeaderCell(string text, float w)
    {
        var style = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft };
        GUILayout.Label(text, style, GUILayout.Width(w));
    }
    private void Cell(string text, float w, bool wrap = false, bool bold = false)
    {
        var st = new GUIStyle(bold ? EditorStyles.boldLabel : EditorStyles.label)
        {
            wordWrap = wrap,
            alignment = TextAnchor.MiddleLeft
        };
        GUILayout.Label(text ?? "-", st, GUILayout.Width(w));
    }
}
