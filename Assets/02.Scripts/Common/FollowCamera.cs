using Unity.Cinemachine;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera cmCam;

    public void SetTarget(Transform target)
    {
        if (!cmCam)
            return;
        cmCam.Target.TrackingTarget = target;
    }
}