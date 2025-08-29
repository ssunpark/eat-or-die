using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatConsoleController : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] private KeyCode _toggleKey = KeyCode.BackQuote; // `
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private TMP_Text _logText;
    [SerializeField] private NetworkPrefabRef _cheatProxyPrefab;

    public static bool IsOpen { get; private set; }

    private readonly List<string> _history = new();
    private int _historyIndex = -1;

    private CheatConsoleExecutor _executor;

    private void Awake()
    {
        if (_panel != null) _panel.SetActive(false);
        Log("[Cheat] Press ` to open. Type 'help' or 'exit'.");
    }
    private void OnEnable()
    {
        if (Room.Instance != null)
        {
            Room.Instance.OnGameStarted += HandleGameStarted;
            if (Room.Instance.Runner != null) HandleGameStarted(Room.Instance.Runner);
        }
        _input.onEndEdit.AddListener(OnEndEdit);
    }
    private void OnDisable()
    {
        if (Room.Instance != null)
            Room.Instance.OnGameStarted -= HandleGameStarted;
        _input.onEndEdit.RemoveListener(OnEndEdit);
    }
    private bool _initialized;
    private void HandleGameStarted(NetworkRunner r)
    {
        StartCoroutine(Co_FindMyExecutor());
    }

    private System.Collections.IEnumerator Co_FindMyExecutor()
    {
        // 네트워크 스폰이 모두 반영될 때까지 잠깐 기다림
        for (int i = 0; i < 120; i++) // 최대 2초 정도
        {
            _executor = FindObjectsByType<CheatConsoleExecutor>(
                FindObjectsInactive.Include, FindObjectsSortMode.None)
                .FirstOrDefault(x => x.Object != null && x.Object.HasInputAuthority);

            if (_executor != null) break;
            yield return null;
        }

        Log(_executor != null ? "[Cheat] Console ready." : "[Cheat] Executor not found (no input authority).");
    }
    private void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
            Toggle();

        if (!IsOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();

        // 히스토리 탐색
        if (_history.Count > 0 && _input != null && _input.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _historyIndex = Mathf.Clamp(_historyIndex + 1, 0, _history.Count - 1);
                _input.text = _history[^(_historyIndex + 1)];
                _input.MoveTextEnd(false);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _historyIndex = Mathf.Clamp(_historyIndex - 1, -1, _history.Count - 1);
                _input.text = _historyIndex < 0 ? "" : _history[^(_historyIndex + 1)];
                _input.MoveTextEnd(false);
            }
        }

        if (IsOpen && _input != null && _input.isFocused &&
        (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            OnSubmit();
            _input.ActivateInputField(); // 계속 입력 이어가려면
        }
    }

    public void Toggle()
    {
        if (_panel == null) return;
        bool next = !_panel.activeSelf;
        _panel.SetActive(next);
        IsOpen = next;

        if (next)
        {
            _input.text = string.Empty;
            _input.ActivateInputField();
            _historyIndex = -1;
        }
    }

    public void Close()
    {
        if (_panel == null) return;
        _panel.SetActive(false);
        IsOpen = false;
    }

    // TMP_InputField onSubmit 이벤트에 바인딩
    public void OnSubmit()
    {
        if (_input == null) return;

        string line = _input.text.Trim();
        if (string.IsNullOrEmpty(line)) return;

        _history.Add(line);
        _historyIndex = -1;
        Log($"> {line}");

        _input.text = string.Empty;
        _input.ActivateInputField();

        // 로컬 제어 커맨드
        if (line.Equals("exit"))
        {
            Close();
            return;
        }
        if (line.Equals("help"))
        {
            PrintHelp();
            return;
        }

        // 나머진 서버에서 실행
        if (_executor == null)
        {
            _executor = FindFirstObjectByType<CheatConsoleExecutor>(FindObjectsInactive.Include);
            if (_executor == null)
            {
                Log("[Cheat] Executor not found.");
                return;
            }
        }
        _executor.TryExecute(line, Log);
    }

    private void PrintHelp()
    {
        Log("Commands:");
        Log("  trait [lv|exp] [TraitEnum] [Value]    - 트레잇 레벨 강제세팅 / 경험치 증가");
        Log("  giveitem [itemId] [qty] [durability]  - 아이템 지급 (durability 기본 1)");
        Log("  tp [x] [y] [z]                         - 지정 위치로 텔레포트 (KCC 사용)");
        Log("  hp [value] / mp [value]                - 체력(허기)/마나 값 설정");
        Log("  enemy [spawn|spawnhere|killall]         - 적 관련");
        Log("  spawner [start|stop|setinterval]        - 스포너 관련");
        Log("  revive [instant]                      - 플레이어 부활 (instant: 트레잇 초기화 포함)");
        Log("  exit                                   - 콘솔 닫기");
    }

    private void Log(string msg)
    {
        if (_logText == null) return;
        _logText.text += (string.IsNullOrEmpty(_logText.text) ? "" : "\n") + msg;
        if (_scroll != null) _scroll.verticalNormalizedPosition = 0;
    }

    private void OnEndEdit(string _)
    {
        OnSubmit();
    }
}
