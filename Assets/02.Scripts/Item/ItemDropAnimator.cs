using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDropAnimator : MonoBehaviour
{
    [SerializeField]
    private float _minJumpForce;

    [SerializeField]
    private float _maxJumpForce;

    public void DropItem(Vector3 target, Action onComplete = null)
    {
        // 1. 수평 이동 (x, z만 이동)
        Vector3 horizontalTarget = new Vector3(target.x, transform.position.y, target.z);
        transform.DOMove(horizontalTarget, 1f).SetEase(Ease.Linear); // x, z만 이동

        // 2. 수직 점프 (위로 상승)
        float jumpHeight = Random.Range(_minJumpForce, _maxJumpForce);
        Sequence jumpSequence = DOTween.Sequence();

        // 위로 상승
        jumpSequence.Append(transform.DOLocalMoveY(transform.position.y + jumpHeight, 0.5f).SetEase(Ease.OutSine));

        // 아래로 하강
        jumpSequence.Append(transform.DOLocalMoveY(target.y, 0.5f).SetEase(Ease.InSine));

        // 3. 착지 후 피드백 실행
        jumpSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}