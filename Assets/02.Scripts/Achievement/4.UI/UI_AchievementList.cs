using System.Collections.Generic;
using UnityEngine;

public class UI_AchievementList : MonoBehaviour
{
    [SerializeField]
    private Transform _content;

    [SerializeField]
    private UI_AchievementSlot _defaultSlotPrefab;
    [SerializeField]
    private UI_AchievementSlot _completeSlotPrefab;

    private readonly Dictionary<int, UI_AchievementSlot> _slotsById = new();
    private IAchievementPresenter _presenter;

    private void Awake()
    {
        _presenter = AchievementManager.Instance.Presenter;
        BuildList(AchievementManager.Instance.GetAchievementDTOList());
    }

    private void OnEnable()
    {
        Refresh(AchievementManager.Instance.GetAchievementDTOList());
        _presenter.OnSnapshot += Refresh;
    }

    private void OnDisable()
    {
        _presenter.OnSnapshot -= Refresh;
    }

    /// 업적 리스트를 생성
    private void BuildList(IReadOnlyList<AchievementViewModel> achievements)
    {
        // 기존 슬롯 제거
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }

        // 업적 순서대로 슬롯 생성
        foreach (var ach in achievements)
        {
            var prefab = ach.IsUnlocked ? _completeSlotPrefab : _defaultSlotPrefab;
            var slot = Instantiate(prefab, _content);
            slot.SetData(ach.Id, ach.Title, ach.Description, ach.Current, ach.Target);
            _slotsById[ach.Id] = slot;
        }
    }

    private void Refresh(IReadOnlyList<AchievementViewModel> achievements)
    {
        foreach (var ach in achievements)
        {
            // 해금인데 Complete가 안된경우 교체
            if (ach.IsUnlocked && !_slotsById[ach.Id].IsCompleted)
            {
                ReplaceWithCompleteSlot(_slotsById[ach.Id], ach);
            }
            var slot = _slotsById[ach.Id];
            slot.ApplyProgress(ach.Current, ach.Target);
        }
    }
    
    private void ReplaceWithCompleteSlot(UI_AchievementSlot oldSlot, AchievementViewModel achievement)
    {
        // 현재 위치 유지
        int index = oldSlot.transform.GetSiblingIndex();
        var parent = oldSlot.transform.parent;

        // 교체
        var newSlot = Instantiate(_completeSlotPrefab, parent);
        newSlot.transform.SetSiblingIndex(index);

        // 기존 텍스트/상태 반영
        newSlot.SetData(achievement.Id, achievement.Title, achievement.Description, achievement.Current, achievement.Target);

        // 맵 갱신
        _slotsById[achievement.Id] = newSlot;

        // 기존 제거
        Destroy(oldSlot.gameObject);
    }
}