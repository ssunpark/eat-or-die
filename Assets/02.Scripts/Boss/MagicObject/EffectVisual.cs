using System;
using UnityEngine;

public class EffectVisual : MonoBehaviour
{
    [SerializeField]
    private bool _isParticleSystem;
    
    [SerializeField]
    private Material _inputMaterial;
    private Material _objectMaterial;
    private MeshRenderer _meshRenderer;
    private ParticleSystemRenderer _particleRenderer;

    [SerializeField]
    private float _stayDuration = 5; // 사라지기 시작까지 대기 시간
    [SerializeField]
    private float _reduceFactor = 1; // 사라지는 속도
    [SerializeField]
    private float _upFactor = 1;     // 나타나는 속도

    private float _time;
    private float _submitReduceFactor;
    private float _cutOutFactor;
    private float _currentUpFactor;
    private bool _isAppearing; // 현재 나타나는 중인지
    
    private event Action OnDespawn;
    public void SetCallBack(Action callback) => OnDespawn = callback;

    private void Awake()
    {
        if (_isParticleSystem)
        {
            _particleRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
            _particleRenderer.material = _inputMaterial;
            _objectMaterial = _particleRenderer.material;
        }
        else
        {
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
            _meshRenderer.material = _inputMaterial;
            _objectMaterial = _meshRenderer.material;
        }
        
        // 테스트
        // Reset(_stayDuration,  _reduceFactor, _upFactor);
    }

    public void Reset(float duration, float reduceFactor, float upFactor)
    {
        _stayDuration = duration;
        _reduceFactor = reduceFactor;
        _upFactor = upFactor;
        _time = 0f;
        _currentUpFactor = 0f;
        _cutOutFactor = 0f;
        _submitReduceFactor = 0f;
        _isAppearing = true;

        _objectMaterial.SetFloat("_MaskCutOut", _cutOutFactor);
    }

    private void LateUpdate()
    {
        if (_isAppearing)
        {
            // 등장 중
            _currentUpFactor += _upFactor * Time.deltaTime;
            _currentUpFactor = Mathf.Clamp01(_currentUpFactor);
            _objectMaterial.SetFloat("_MaskCutOut", _currentUpFactor);

            if (_currentUpFactor >= 1f)
            {
                _isAppearing = false;
                _cutOutFactor = 1f; // 완전히 보인 상태로 고정
                _time = 0f;         // 사라지기까지의 대기 시간 타이머 시작
            }

            return;
        }

        // 사라짐 대기 타이머
        _time += Time.deltaTime;
        if (_time > _stayDuration)
        {
            _submitReduceFactor = Mathf.Lerp(_submitReduceFactor, _reduceFactor, Time.deltaTime / 50f);
            _cutOutFactor -= _submitReduceFactor * Time.deltaTime;
            _cutOutFactor = Mathf.Clamp01(_cutOutFactor);

            _objectMaterial.SetFloat("_MaskCutOut", _cutOutFactor);
            if (_cutOutFactor <= 0)
            {
                OnDespawn?.Invoke();
            }
        }
    }
}