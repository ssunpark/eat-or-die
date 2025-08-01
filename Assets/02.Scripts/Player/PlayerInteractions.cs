using System;
using Fusion;
using UnityEngine;

// 플레이어 상호작용
public class PlayerInteractions : MonoBehaviour
{

    public LayerMask InteractionLayer;
    public string TagName;
    public int HoldItemID;
    public string ItemName;
    public PlayerController _controller; // 플레이어 컨트롤러 참조

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        InteractionLayer = LayerMask.GetMask("Interactable");
        TagName = "Untagged";
    }
    public void Interact(GameObject target)
    {
        target.GetComponent<IInteractable>()?.Interact();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 상호작용 범위를 파란색 구체로 그립니다.
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }

    
#endif
}