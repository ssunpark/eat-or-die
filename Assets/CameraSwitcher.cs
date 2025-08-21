using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase[] _vcams;
    private int _currentIndex = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCamera(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCamera(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCamera(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchCamera(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchCamera(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SwitchCamera(5);
    }

    private void SwitchCamera(int index)
    {
        if (index == _currentIndex) return;

        // 모든 카메라 Priority 낮춤
        for (int i = 0; i < _vcams.Length; i++)
        {
            _vcams[i].Priority = 0;
            var dolly = _vcams[i].GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineSplineDolly;

            if (dolly == null) continue;


        }

        // 선택한 카메라 Priority 높임
        _vcams[index].Priority = 100;
        _currentIndex = index;
    }
}
