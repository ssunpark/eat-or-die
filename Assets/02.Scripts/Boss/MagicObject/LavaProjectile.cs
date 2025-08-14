using System;
using DG.Tweening;
using Fusion;
using Redcode.Pools;
using UnityEngine;

public struct LavaProjectileData
{
    public readonly Vector3 TargetPosition;
    public readonly float Speed;
    public readonly float Duration;
    public readonly float Height;

    public LavaProjectileData(Vector3 targetPosition, float speed, float duration, float height)
    {
        TargetPosition = targetPosition;
        Speed = speed;
        Duration = duration;
        Height = height;
    }
}

public class LavaProjectile : NetworkBehaviour
{
    // 네트워크로 공유되는 초기값(스폰 시에만 셋)
    [Networked]
    public Vector3 StartPosition { get; set; }
    [Networked]
    public Vector3 TargetPos { get; set; }
    [Networked]
    public float Speed { get; set; }
    [Networked]
    public float Height { get; set; } // 포물선 높이
    [Networked]
    public float Duration { get; set; } // 도착 후 장판 지속시간(다음 단계에서 사용)
    [Networked]
    public int StartTick { get; set; } // 네트 시작 틱 (시간 보정용)

    // 로컬 상태
    private bool _arrived;
    private float _travelTime;          // distance / Speed
    private Action _onArrivedAuthority; // 권위에서만 설정/호출
    
    public float _damage;
    public void SetDamage(float damage) => _damage = damage;

    public void SetArrivedAction(Action callback)
    {
        if (!HasStateAuthority)
        {
            return;
        }
        
        _onArrivedAuthority = callback;
    }

    public override void Spawned()
    {
        transform.position = StartPosition;
        
        // 프록시/권위 모두 동일하게 이동을 "재현"할 수 있도록 travelTime 계산
        _travelTime = Vector3.Distance(StartPosition, TargetPos) / Mathf.Max(0.01f, Speed);

        // Y값 무시하고 바라보는 방향 계산
        Vector3 dir = TargetPos - StartPosition;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized);
        else
            transform.rotation = Quaternion.identity; // 동일 위치 방어

        _arrived = false;
    }

    public override void Render()
    {
        // 네트워크 시간 기준 진행도
        float elapsed = (Runner.Tick - StartTick) * Runner.DeltaTime;
        if (elapsed < 0f)
            return; // 아직 시작 전

        float t = _travelTime <= 0f ? 1f : Mathf.Clamp01(elapsed / _travelTime);

        // 포물선 위치 계산 (Lerp + 간단한 포물선 높이)
        Vector3 p = Vector3.Lerp(StartPosition, TargetPos, t);
        // h(t) = 4 * H * t * (1 - t)  (정점에서 Height 만큼 상승)
        float h = 4f * Height * t * (1f - t);
        p.y += h;
        transform.position = p;

        // 도착 처리 (한 번만)
        if (!_arrived && t >= 1f)
        {
            _arrived = true;

            if (Object.HasStateAuthority)
            {
                // 권위에서만 도착 콜백 (장판 생성 등)
                _onArrivedAuthority?.Invoke();
            }

            // 살짝 여유 두고 디스폰
            Runner.Despawn(Object);
        }
    }

    // 충돌 판정은 권위만 수행(선택)
    private void OnTriggerEnter(Collider other)
    {
        if (!Object || !Object.HasStateAuthority)
            return;
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent(out IAttackable hit))
        {
            hit.OnHitLocal(new AttackInfo { MeleeDamage = _damage, TotalDamageMultiplier = 1f });
        }
    }
}