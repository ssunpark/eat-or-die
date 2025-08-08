using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UI_PartySlot : MonoBehaviour
{
    [SerializeField] private Slider _partySlider;
    [SerializeField] private TextMeshProUGUI _playerNicknameText;

    private PlayerRef _boundPlayer;
    public PlayerRef BoundPlayer => _boundPlayer;

    public void Bind(PlayerRef playerRef, string nickname)
    {
        _boundPlayer = playerRef;
        gameObject.SetActive(true);
        SetNickName(nickname);
    }

    public void ResetSlot()
    {
        _boundPlayer = PlayerRef.None;
        ResetAll();
        gameObject.SetActive(false);
    }

    public string GetNickname() => _playerNicknameText.text;
    public void SetNickName(string nickname)
    {
        _playerNicknameText.text = nickname;
    }

    public void SetSliderValue(float currentValue, float maxValue)
    {
        if (_partySlider != null)
        {
            if (maxValue <= 0)
            {
                _partySlider.value = 0;
            }
            else
            {
                _partySlider.value = currentValue / maxValue;
            }
        }
    }

    public void ResetAll()
    {
        if (_playerNicknameText != null)
        {
            _playerNicknameText.text = string.Empty;
        }
        if (_partySlider != null)
        {
            _partySlider.value = 0;
        }
    }
}
