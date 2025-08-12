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

    // Reflection 캐시 (바뀐 매니저 필드에 맞춤)
    private FieldInfo _fiSkills;    // Dictionary<int, Skill>
    private FieldInfo _fiHandlers;  // Dictionary<int, ISkillHandler>

    // 데이터 스냅샷 (표시에만 사용)
    private List<(int id, string name)> _dbView = new();
    private List<(int id, int level, string runtimeName)> _activeView = new();

    [MenuItem("Tools/Skill Manager Window")]
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
        RefreshSnapshots(force: true);
    }

    private void OnEditorUpdate()
    {
        if (_autoRefresh && EditorApplication.isPlaying)
        {
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
        EditorGUILayout.LabelField("스킬 DB (SkillManager._skills)", EditorStyles.boldLabel);
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
                    GUILayout.Label(string.IsNullOrEmpty(rname) ? "(Runtime 없음)" : rname);
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
        _fiSkills   = smType.GetField("_skills",   BindingFlags.Instance | BindingFlags.NonPublic);
        _fiHandlers = smType.GetField("_handlers", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    private SkillManager GetSkillManagerInstance()
    {
        // BehaviourSingleton.Instance가 없을 수 있으니 보조로 FindObjectOfType 사용
        var inst = FindObjectOfType<SkillManager>();
        return inst;
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // 내부: 스냅샷 생성 (바뀐 구조에 맞게)
    // ──────────────────────────────────────────────────────────────────────────────
    private void RefreshSnapshots(bool force)
    {
        var mgr = GetSkillManagerInstance();
        _dbView.Clear();
        _activeView.Clear();

        if (!mgr) return;

        try
        {
            // _skills: Dictionary<int, Skill>
            var skillsDict = _fiSkills?.GetValue(mgr) as IDictionary;
            // _handlers: Dictionary<int, ISkillHandler> (활성 런타임만)
            var handlersDict = _fiHandlers?.GetValue(mgr) as IDictionary;

            if (skillsDict != null)
            {
                foreach (DictionaryEntry kv in skillsDict)
                {
                    int id = (int)kv.Key;
                    object skillObj = kv.Value;

                    // Skill.Meta에서 이름 뽑기
                    string name = ExtractNameFromSkill(skillObj);

                    // DB 뷰(좌측)
                    _dbView.Add((id, name));

                    // 활성 뷰(우측): Level > 0
                    var level = ExtractLevelFromSkill(skillObj);
                    if (level > 0)
                    {
                        string runtimeName = "";
                        if (handlersDict != null && handlersDict.Contains(id))
                        {
                            var handler = handlersDict[id];
                            runtimeName = handler != null ? handler.GetType().Name : "";
                        }
                        _activeView.Add((id, level, runtimeName));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private int ExtractLevelFromSkill(object skillObj)
    {
        if (skillObj == null) return 0;
        var t = skillObj.GetType();
        var pLevel = t.GetProperty("Level", BindingFlags.Public | BindingFlags.Instance);
        if (pLevel != null && pLevel.PropertyType == typeof(int))
        {
            try { return (int)pLevel.GetValue(skillObj); }
            catch { /* ignore */ }
        }
        return 0;
    }

    private string ExtractNameFromSkill(object skillObj)
    {
        if (skillObj == null) return "";
        var t = skillObj.GetType();
        var pMeta = t.GetProperty("Meta", BindingFlags.Public | BindingFlags.Instance);
        if (pMeta == null) return "";

        var meta = pMeta.GetValue(skillObj);
        if (meta == null) return "";

        // SkillRawData 안의 이름 필드 추정
        var mt = meta.GetType();
        var pName = mt.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
        if (pName != null && pName.PropertyType == typeof(string)) return (string)pName.GetValue(meta);

        var pTitle = mt.GetProperty("Title", BindingFlags.Public | BindingFlags.Instance);
        if (pTitle != null && pTitle.PropertyType == typeof(string)) return (string)pTitle.GetValue(meta);

        var pDisp = mt.GetProperty("DisplayName", BindingFlags.Public | BindingFlags.Instance);
        if (pDisp != null && pDisp.PropertyType == typeof(string)) return (string)pDisp.GetValue(meta);

        return "";
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
