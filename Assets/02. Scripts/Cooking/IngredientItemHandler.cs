using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class IngredientItemHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public IngredientData data;
    // public Image iconImage;

    private Transform originalParent;

    public void Init(IngredientData newData)
    {
        data = newData;
        // iconImage.sprite = data.Icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.parent == transform.root)
        {
            transform.SetParent(originalParent);
        }
    }
}
