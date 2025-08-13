using System;
using UnityEngine;

public class DirectionalProjectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private float _lifeTime;
    private float _timer;
    private float _damage;
    private bool _isFired = false;

    private Action _endCallback;

    // Fire 메서드: 방향, 속도, 생존 시간, 생존 시간 이후 콜백 설정
    public void Fire(Vector3 direction, float speed, float lifeTime, float damage, Action endCallback)
    {
        _direction = direction.normalized;
        var angle = Quaternion.LookRotation(_direction.normalized).eulerAngles.y;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        _speed = speed;
        _lifeTime = lifeTime;
        _damage = damage;
        _timer = 0f;
        _isFired = true;
        _endCallback = endCallback;
    }

    public void Fire(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        var angle = Quaternion.LookRotation(_direction.normalized).eulerAngles.y;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
        _lifeTime = -1f;
        _speed = speed;
        _timer = 0f;
        _isFired = true;
    }

    void Update()
    {
        if (!_isFired)
            return;

        // 이동
        transform.position += _direction * _speed * Time.deltaTime;

        // 생존 시간 체크
        if (_lifeTime < 0)
        {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            if (_endCallback != null)
            {
                _endCallback.Invoke();
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
                MeleeDamage = _damage,
                TotalDamageMultiplier = 1f
            };
            hit.OnHitLocal(attackinfo);
        }
    }
}