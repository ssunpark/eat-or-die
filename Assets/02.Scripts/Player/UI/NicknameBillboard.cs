using UnityEngine;

public class NicknameBillboard : MonoBehaviour
{
    private Transform _camTransform;

    private void Start()
    {
        // 로컬 플레이어의 카메라를 찾음 (자신의 로컬 카메라)
        if (Camera.main != null)
        {
            _camTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (_camTransform == null) return;

        // 카메라를 바라보게 회전
        transform.rotation = Quaternion.LookRotation(transform.position - _camTransform.position);
    }
}
