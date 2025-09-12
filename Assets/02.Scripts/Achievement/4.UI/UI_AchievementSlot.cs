using TMPro;
using UnityEngine;

public class UI_AchievementSlot : MonoBehaviour
{
    [SerializeField]
    private bool _isCompleted;
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _descriptionText;
    [SerializeField]
    private TextMeshProUGUI _currentText;
    [SerializeField]
    private TextMeshProUGUI _targetText;

    public int Id { get; private set; }
    public bool IsCompleted => _isCompleted;

    /// <summary>
    /// 업적 데이터로 슬롯 갱신
    /// </summary>
    public void SetData(int id, string title, string description, long current, long target)
    {
        Id = id;
        _titleText.text = title;
        _descriptionText.text = description;

        ApplyProgress(current, target);
    }

    public void ApplyProgress(long current, long target)
    {
        _currentText.text = current.ToString();
        _targetText.text = target.ToString();
    }
}