using UnityEngine;

public class CheatConsoleEnabler : MonoBehaviour
{
    [SerializeField] private GameObject _consoleRoot; // CheatConsole 루트 (비활성로 시작)

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (_consoleRoot != null && !_consoleRoot.activeSelf)
                _consoleRoot.SetActive(true); // 이때부터 Controller가 Update를 돌기 시작
        }
    }
}
