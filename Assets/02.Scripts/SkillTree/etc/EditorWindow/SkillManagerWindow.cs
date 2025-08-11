#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SkillManagerWindow : EditorWindow
{
    // --- UI State ---
    private Vector2 _leftScroll;
    private Vector2 _rightScroll;
    private string _search = "";
    private int _inputId = 0;
    private int _inputLevel = 1;
    private double _lastAutoRefresh;
    private bool _autoRefresh = true;

    // Reflection 캐시
    private FieldInfo _fiRawCache;
    private FieldInfo _fiActiveSkills;
    private PropertyInfo _piRuntimeLevel;

    // 데이터 스냅샷 (표시에만 사용)
    private List<(int id, string name)> _dbView = new();
    private List<(int id, int level, string runtimeName)> _activeView = new();

    [MenuItem("Tools/Skills/Skill Manager Window")]
    private static void Open()
    {
        var win = GetWindow<SkillManagerWindow>("Skill Manager");
        win.Show();
    }

    private void OnEnable()
    {
        TryBindReflection();
        RefreshSnapshots(force: true);
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnPlayModeChanged(PlayModeStateChange change)
    {
        // 플레이 모드 전환 시 리플레시
        RefreshSnapshots(force: true);
    }

    private void OnEditorUpdate()
    {
        if (_autoRefresh && EditorApplication.isPlaying)
        {
            // 너무 자주 리프레시하지 않도록 0.25s 쿨다운
            if (EditorApplication.timeSinceStartup - _lastAutoRefresh > 0.25f)
            {
                _lastAutoRefresh = EditorApplication.timeSinceStartup;
                RefreshSnapshots(force: false);
                Repaint();
            }
        }
    }

    private void OnGUI()
    {
        DrawHeader();
        EditorGUILayout.Space(4);

        if (!EditorApplication.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 등록/업그레이드/해제가 실제로 실행됩니다. (표시는 미리보기 가능)", MessageType.Info);

        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(position.width * 0.48f)))
            {
                DrawDatabasePanel();
            }

            GUILayout.Space(8);

            using (new EditorGUILayout.VerticalScope())
            {
                DrawActivePanel();
            }
        }

        EditorGUILayout.Space(8);
        DrawControlsPanel();
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // UI: 상단 헤더
    // ──────────────────────────────────────────────────────────────────────────────
    private void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            _search = GUILayout.TextField(_search, EditorStyles.toolbarSearchField, GUILayout.MinWidth(120));
            GUILayout.FlexibleSpace();

            _autoRefresh = GUILayout.Toggle(_autoRefresh, "Auto Refresh (Play)", EditorStyles.toolbarButton, GUILayout.Width(140));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                RefreshSnapshots(force: true);
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // UI: 좌측 - 스킬 DB 목록(이름/ID)
    // ──────────────────────────────────────────────────────────────────────────────
    private void DrawDatabasePanel()
    {
        EditorGUILayout.LabelField("스킬 DB (CSV 캐시)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("SkillManager의 _rawDataCache를 reflection으로 조회하여 표시합니다.", MessageType.None);

        using (var scroll = new EditorGUILayout.ScrollViewScope(_leftScroll, GUILayout.Height(position.height * 0.55f)))
        {
            _leftScroll = scroll.scrollPosition;

            if (_dbView.Count == 0)
            {
                EditorGUILayout.LabelField("표시할 데이터가 없습니다. (플레이 모드에서 SkillManager.Awake가 실행되어야 합니다)");
                return;
            }

            foreach (var (id, name) in FilterDB(_dbView))
            {
                using (new EditorGUILayout.HorizontalScope("box"))
                {
                    GUILayout.Label($"ID: {id}", GUILayout.Width(90));
                    GUILayout.Label(string.IsNullOrEmpty(name) ? "(Name 없음)" : name);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("선택", GUILayout.Width(60)))
                    {
                        _inputId = id;
                    }
                }
            }
        }
    }

    private IEnumerable<(int id, string name)> FilterDB(IEnumerable<(int id, string name)> src)
    {
        if (string.IsNullOrWhiteSpace(_search)) return src;
        var s = _search.Trim();
        return src.Where(x =>
            x.id.ToString().IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0 ||
            (x.name ?? "").IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // UI: 우측 - 현재 활성 스킬 목록(레벨)
    // ──────────────────────────────────────────────────────────────────────────────
    private void DrawActivePanel()
    {
        EditorGUILayout.LabelField("현재 활성 스킬", EditorStyles.boldLabel);

        using (var scroll = new EditorGUILayout.ScrollViewScope(_rightScroll, GUILayout.Height(position.height * 0.55f)))
        {
            _rightScroll = scroll.scrollPosition;

            if (_activeView.Count == 0)
            {
                EditorGUILayout.LabelField("(활성 스킬 없음)");
                return;
            }

            foreach (var (id, level, rname) in _activeView.OrderBy(x => x.id))
            {
                using (new EditorGUILayout.HorizontalScope("box"))
                {
                    GUILayout.Label($"ID: {id}", GUILayout.Width(80));
                    GUILayout.Label($"Lv.{level}", GUILayout.Width(70));
                    GUILayout.Label(string.IsNullOrEmpty(rname) ? "(Runtime)" : rname);
                    GUILayout.FlexibleSpace();

                    using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
                    {
                        if (GUILayout.Button("해제", GUILayout.Width(60)))
                            CallInactive(id);

                        if (GUILayout.Button("+1", GUILayout.Width(40)))
                            CallUpgrade(id, level + 1);
                    }
                }
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // UI: 하단 - 조작 영역(등록/업그레이드/해제)
    // ──────────────────────────────────────────────────────────────────────────────
    private void DrawControlsPanel()
    {
        EditorGUILayout.LabelField("조작", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope("box"))
        {
            _inputId = EditorGUILayout.IntField(new GUIContent("Skill ID"), _inputId, GUILayout.MaxWidth(260));
            _inputLevel = EditorGUILayout.IntField(new GUIContent("Level"), Mathf.Max(1, _inputLevel), GUILayout.MaxWidth(220));
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
            {
                if (GUILayout.Button("등록/갱신 (Active)", GUILayout.Height(22)))
                    CallActive(_inputId, _inputLevel);

                if (GUILayout.Button("업그레이드 (Upgrade)", GUILayout.Height(22)))
                    CallUpgrade(_inputId, _inputLevel);

                if (GUILayout.Button("해제 (Inactive)", GUILayout.Height(22)))
                    CallInactive(_inputId);
            }
        }
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // 내부: 리플렉션 바인딩
    // ──────────────────────────────────────────────────────────────────────────────
    private void TryBindReflection()
    {
        var smType = typeof(SkillManager);
        _fiRawCache = smType.GetField("_rawDataCache", BindingFlags.Instance | BindingFlags.NonPublic);
        _fiActiveSkills = smType.GetField("_activeSkills", BindingFlags.Instance | BindingFlags.NonPublic);

        // IRuntimeSkill.Level (public get) 추정
        // 런타임 타입을 모를 수 있으므로 런타임에 발견 시 캐시
        _piRuntimeLevel = null;
    }

    private SkillManager GetSkillManagerInstance()
    {
        // BehaviourSingleton<SkillManager>.Instance 를 못쓸 수도 있으니 FindObjectOfType로 보조
        var inst = FindObjectOfType<SkillManager>();
        return inst;
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // 내부: 스냅샷 생성
    // ──────────────────────────────────────────────────────────────────────────────
    private void RefreshSnapshots(bool force)
    {
        var mgr = GetSkillManagerInstance();
        _dbView.Clear();
        _activeView.Clear();

        if (!mgr)
            return;

        try
        {
            // DB (id, name)
            var rawDictObj = _fiRawCache?.GetValue(mgr) as IDictionary;
            if (rawDictObj != null)
            {
                foreach (DictionaryEntry kv in rawDictObj)
                {
                    int id = (int)kv.Key;
                    string name = ExtractNameFromRaw(kv.Value);
                    _dbView.Add((id, name));
                }
            }

            // Active (id, level, runtimeName)
            var activeDictObj = _fiActiveSkills?.GetValue(mgr) as IDictionary;
            if (activeDictObj != null)
            {
                foreach (DictionaryEntry kv in activeDictObj)
                {
                    int id = (int)kv.Key;
                    object runtimeSkill = kv.Value;

                    int level = ExtractLevelFromRuntime(runtimeSkill);
                    string rname = runtimeSkill != null ? runtimeSkill.GetType().Name : "";
                    _activeView.Add((id, level, rname));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private string ExtractNameFromRaw(object raw)
    {
        if (raw == null) return "";
        // SkillRawData 안에 Name, Title, DisplayName 같은 속성이 있을 수 있으니 우선순위로 시도
        var t = raw.GetType();
        var pName = t.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
        if (pName != null && pName.PropertyType == typeof(string)) return (string)pName.GetValue(raw);

        var pTitle = t.GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);
        if (pTitle != null && pTitle.PropertyType == typeof(string)) return (string)pTitle.GetValue(raw);

        var pDisp = t.GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance);
        if (pDisp != null && pDisp.PropertyType == typeof(string)) return (string)pDisp.GetValue(raw);

        return ""; // 이름 정보가 없을 수 있음
    }

    private int ExtractLevelFromRuntime(object runtimeSkill)
    {
        if (runtimeSkill == null) return 0;

        if (_piRuntimeLevel == null)
        {
            // 최초 한 번만 Level 프로퍼티를 찾아 캐시
            _piRuntimeLevel = runtimeSkill.GetType().GetProperty("Level", BindingFlags.Public | BindingFlags.Instance);
        }

        if (_piRuntimeLevel != null && _piRuntimeLevel.PropertyType == typeof(int))
        {
            try { return (int)_piRuntimeLevel.GetValue(runtimeSkill); }
            catch { /* ignore */ }
        }

        // Level이 없을 경우 0 처리
        return 0;
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // 내부: 호출 (Active/Upgrade/Inactive)
    // ──────────────────────────────────────────────────────────────────────────────
    private void CallActive(int id, int level)
    {
        var mgr = GetSkillManagerInstance();
        if (!mgr) { ShowNotify("SkillManager 인스턴스를 찾지 못했습니다."); return; }

        Undo.RecordObject(mgr, "Skill Active");
        try
        {
            mgr.Active(id, level);
            EditorUtility.SetDirty(mgr);
            ShowNotify($"Active: ID {id}, Lv {level}");
            RefreshSnapshots(force: true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowNotify("Active 실패 (Console 확인)");
        }
    }

    private void CallUpgrade(int id, int newLevel)
    {
        var mgr = GetSkillManagerInstance();
        if (!mgr) { ShowNotify("SkillManager 인스턴스를 찾지 못했습니다."); return; }

        Undo.RecordObject(mgr, "Skill Upgrade");
        try
        {
            mgr.Upgrade(id, newLevel);
            EditorUtility.SetDirty(mgr);
            ShowNotify($"Upgrade: ID {id} -> Lv {newLevel}");
            RefreshSnapshots(force: true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowNotify("Upgrade 실패 (Console 확인)");
        }
    }

    private void CallInactive(int id)
    {
        var mgr = GetSkillManagerInstance();
        if (!mgr) { ShowNotify("SkillManager 인스턴스를 찾지 못했습니다."); return; }

        Undo.RecordObject(mgr, "Skill Inactive");
        try
        {
            mgr.Inactive(id);
            EditorUtility.SetDirty(mgr);
            ShowNotify($"Inactive: ID {id}");
            RefreshSnapshots(force: true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ShowNotify("Inactive 실패 (Console 확인)");
        }
    }

    private void ShowNotify(string msg)
    {
        this.ShowNotification(new GUIContent(msg));
    }
}
#endif
