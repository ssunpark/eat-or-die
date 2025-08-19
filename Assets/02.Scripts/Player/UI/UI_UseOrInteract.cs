using TMPro;
using UnityEngine;
// 스크립트
public class UI_UseOrInteract : MonoBehaviour
{
    [SerializeField] private bool _isUse = true; // true: 사용, false: 상호작용

    private string _objectName = "몰?루";// 객체 이름
    private string _actionName = "몰?루";// 액션 이름

    [SerializeField] private TextMeshProUGUI _objectNameText;
    [SerializeField] private CanvasGroup _alphaControl;
    [SerializeField] private TextMeshProUGUI _actionText;
    public string ObjectName
    {
        get { return _objectName; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _objectName = "";
                _objectNameText.text = "몰?루";
                _alphaControl.alpha = 0f;
            }
            else
            {
                _objectName = value;
                _objectNameText.text = value;
                _alphaControl.alpha = 1f;
            }
        }
    }

    public string ActionName
    {
        get { return _actionName; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _actionName = "";
                _actionText.text = "무언가를 하기";
            }
            else
            {
                _actionName = value;
                _actionText.text = value;
            }
        }
    }

    private GameObject _targetObject;
    public GameObject TargetObject
    {
        get { return _targetObject; }
        set
        {
            _targetObject = value;
            if (_targetObject != null)
            {
                if (_targetObject.TryGetComponent(out UI_NameTag nameTag))
                {
                    ObjectName = nameTag.ObjName;
                    ActionName = nameTag.ActName;
                }
                else
                {
                    ObjectName = "안붙이고 모하냐 이놈아~";
                    ActionName = "이름표를 붙여라";
                }
                Vector3 useOffset = nameTag?.UseOffset?? new Vector3(2f, 2f, -1f);
                Vector3 interactOffset = nameTag?.InteractOffset ?? new Vector3(-2f, 2f, -1f);
                Vector3 pos = _targetObject.transform.position + (_isUse ? useOffset : interactOffset);
                transform.position = pos;
            }
            else
            {
                ObjectName = "";
            }
        }
    }
}
