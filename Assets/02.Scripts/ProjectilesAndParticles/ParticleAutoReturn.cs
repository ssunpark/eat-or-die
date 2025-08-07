using UnityEngine;
using Redcode.Pools;

public class ParticleAutoReturn : MonoBehaviour
{
    private ParticleSystem _ps;
    private bool _isPlaying;
    private Pool<ParticleSystem> _pool;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _pool = ProjectileManager.Instance.ExplodeEffectPool[_ps];
    }

    private void OnEnable()
    {
        _ps.Play();
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying && !_ps.IsAlive())
        {
            _isPlaying = false;

            if (_pool != null)
            {
                _pool.Take(_ps);
            }
            else
            {
                Destroy(_ps.gameObject);
            }
        }
    }
}
