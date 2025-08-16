using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RiviveSelect : MonoBehaviour
{
    private float _timer;
    private const float _maxTime = 60f;
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
        _instantRiviveBtn.onClick.AddListener(OnInstantRivive);
        _waitRiviveBtn.onClick.AddListener(OnWaitRivive);
        _timerText.text = _timer.ToString("F0");
    }

    private void OnInstantRivive()
    {
        _player.InstantRevive();
        Destroy(gameObject);
    }

    private void OnWaitRivive()
    {
        _player.RequestState(EPlayerState.Corpse);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (_player == null || !_player.IsDead) return;
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = 0f;
            OnWaitRivive();
        }
        _timerText.text = _timer.ToString("F0");
    }
}
