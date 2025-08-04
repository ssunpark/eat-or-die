using DG.Tweening;
using UnityEngine;

public class LavaProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private float _duration;
    private float _height;

    private Tween _moveTween;

    /// <summary>
    /// 포물선 이동 시작을 위한 초기화 함수
    /// </summary>
    /// <param name="targetPosition">목표 지점</param>
    /// <param name="duration">도달 시간</param>
    /// <param name="height">포물선 높이</param>
    public void Fire(Vector3 targetPosition, float duration, float height)
    {
        _targetPosition = targetPosition;
        _duration = duration;
        _height = height;

        StartParabolaMove();
    }

    private void StartParabolaMove()
    {
        Vector3 startPos = transform.position;

        // 포물선 정점
        Vector3 midPoint = (startPos + _targetPosition) / 2;
        midPoint.y += _height;

        // 경로: 중간 → 목표
        Vector3[] path = new Vector3[] { midPoint, _targetPosition };

        // 기존 Tween 정리
        _moveTween?.Kill();

        _moveTween = transform
            .DOPath(path, _duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(OnArrived);
    }

    private void OnArrived()
    {
        Debug.Log("도착 완료!");
        // 이펙트나 데미지 처리 등 추가 가능
    }
}