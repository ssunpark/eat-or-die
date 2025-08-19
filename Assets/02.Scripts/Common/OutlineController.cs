using EPOOutline;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    [SerializeField]
    private Outlinable _outlineObject;
    public Outlinable OutlineObject
    {
        get => _outlineObject;
        set
        {
            if (value == null)
            {
            }
            _outlineObject = value;
        }
    }


    public void SetOutlineActive(bool active)
    {
        if (enabled == false) return;
        if (_outlineObject == null)
        {
            Debug.LogWarning($"{name} 아웃라인 넣어주세요");
            return;
        }
        _outlineObject.enabled = active;
    }

    public void ActiveOutline() => SetOutlineActive(true);
    public void InactiveOutline() => SetOutlineActive(false);
}