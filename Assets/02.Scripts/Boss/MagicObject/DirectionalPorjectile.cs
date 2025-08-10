using System;
using UnityEngine;

public class DirectionalProjectile : MonoBehaviour
{
    private const float DEFAULT_LIFRTIME = 10f;
    
    private Vector3 _direction;
    private float _speed;
    private float _lifeTime;
    private float _timer;
    private bool _isFired = false;

    private Action _callback;

    // Fire 메서드: 방향, 속도, 생존 시간, 생존 시간 이후 콜백 설정
    public void Fire(Vector3 direction, float speed, float lifeTime = DEFAULT_LIFRTIME, Action callback = null)
    {
        _direction = direction.normalized;
        var angle = Quaternion.LookRotation(_direction.normalized).eulerAngles.y;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        _speed = speed;
        _lifeTime = lifeTime;
        _timer = 0f;
        _isFired = true;
        _callback = callback;
    }

    void Update()
    {
        if (!_isFired)
            return;

        // 이동
        transform.position += _direction * _speed * Time.deltaTime;

        // 생존 시간 체크
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            if (_callback != null)
            {
                _callback.Invoke();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        var hit = other.GetComponent<IAttackable>();
        if (hit != null)
        {
            var attackinfo = new AttackInfo
            {
                MeleeDamage = 10f,
                TotalDamageMultiplier = 1f
            };
            hit.OnHitLocal(attackinfo);
        }
    }
}