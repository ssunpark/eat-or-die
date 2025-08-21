using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines; // PathIndexUnit
using Unity.Mathematics;

[DisallowMultipleComponent]
public class SplineCameraDriver : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase _vCam;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private bool _useNormalized = true;
    [SerializeField] private bool _playOnStart = true;

    private CinemachineSplineDolly _dolly;
    private float _pos;
    private bool _isPlaying;

    private void Awake()
    {
        if (_vCam == null) _vCam = GetComponent<CinemachineVirtualCameraBase>();
        _dolly = _vCam?.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineSplineDolly;
        if (_dolly == null) { Debug.LogError("Body를 Spline Dolly로 설정하세요."); enabled = false; return; }

        _dolly.PositionUnits = _useNormalized ? PathIndexUnit.Normalized : PathIndexUnit.Distance;
        _isPlaying = _playOnStart;
        _pos = _dolly.CameraPosition; // ← 새 API
    }

    private void Update()
    {
        if (_isPlaying) Step(Time.deltaTime);
        if (Input.GetKey(KeyCode.PageUp)) Step(-Time.deltaTime);
        if (Input.GetKey(KeyCode.PageDown)) Step(+Time.deltaTime);
    }

    private void Step(float dt)
    {
        if (_dolly.Spline == null) return;

        float delta = _speed * dt;
        if (_useNormalized)
        {
            switch (_dolly.PositionUnits)
            {
                case PathIndexUnit.Normalized:
                    // WrapMode는 Dolly가 내부적으로 처리하지만, 수동 루프가 필요하면 아래처럼
                    _pos = Mathf.Repeat(_pos + delta, 1f);
                    break;
            }
        }
        else
        {
            // _dolly.Spline : SplineContainer
            var spline = _dolly.Spline.Spline; // Spline (곡선 데이터)
            float4x4 trs = (float4x4)_dolly.Spline.transform.localToWorldMatrix; // ★ 변환 행렬

            float length = SplineUtility.CalculateLength(spline, trs); // ★ 이렇게 호출
            _pos += delta;

            if (length > 0f)
            {
                _pos = Mathf.Clamp(_pos, 0f, length);
            }
        }

        _dolly.CameraPosition = _pos;
    }

    public void Play() => _isPlaying = true;
    public void Pause() => _isPlaying = false;

    public void SetPosition(float position)
    {
        _pos = position;
        if (_dolly != null) _dolly.CameraPosition = _pos;
    }

    public void SetNormalizedMode(bool useNormalized)
    {
        _useNormalized = useNormalized;
        if (_dolly == null) return;
        _dolly.PositionUnits = _useNormalized ? PathIndexUnit.Normalized : PathIndexUnit.Distance;
    }
}
