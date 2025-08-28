using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ReviveSelect : MonoBehaviour
{
    private float _timer;
    private const float _maxTime = 5f;
    [SerializeField] private Button _instantRiviveBtn;
    [SerializeField] private Button _waitRiviveBtn;
    [SerializeField] private TextMeshProUGUI _timerText;
    private Player _player;
    private PlayerFSM _playerFSM;

    public void Initialize(PlayerFSM playerFSM,Player player)
    {
        _timer = _maxTime;
        _player = player;
        _playerFSM = playerFSM;
        _instantRiviveBtn.onClick.AddListener(OnInstantRevive);
        _waitRiviveBtn.onClick.AddListener(OnWaitRevive);
        _timerText.text = _timer.ToString("F0");

        _player.OnRevive += HandleRevived;
    }

    private void HandleRevived()
    {
        _player.OnRevive -= HandleRevived;
        Destroy(gameObject);
    }

    private void OnInstantRevive()
    {
        _player.RPC_RequestInstantRevive();
    }

    private void OnWaitRevive()
    {
        _player.RequestState(EPlayerState.Corpse);
        _player.OnRevive -= HandleRevived;
        Destroy(gameObject);
    }

    private void Update()
    {
        if (_player == null || !_player.IsDead) return;
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = 0f;
            OnWaitRevive();
        }
        _timerText.text = _timer.ToString("F0");
    }
}
