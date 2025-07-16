using UnityEngine;

// 필드에서 줍기 가능한 오브젝트에 대한 인터페이스
public interface IPickable
{
    public void Pick(GameObject target);
}