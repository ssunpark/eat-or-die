using DG.Tweening;
using UnityEngine;
// 스크립트
public class DragonFly : MonoBehaviour
{
    private void Start()
    {

        transform.DOMove(transform.forward * 600, 80f);
    }
}
