using UnityEngine;
public class UI_NameTag : MonoBehaviour
{
    [SerializeField] private string _objName;
    [SerializeField] private string _actName;
    [SerializeField] private PlayerCustomizeHandler _playerCustomizeHandler;
    [SerializeField] private Vector3 _useOffset = new Vector3(2f, 2f, -1f); 
    [SerializeField] private Vector3 _interactOffset = new Vector3(-2f, 2f, -1f);

    public Vector3 UseOffset => _useOffset;
    public Vector3 InteractOffset => _interactOffset;
    public string ObjName
    {
        get {
            if (string.IsNullOrEmpty(_objName))
            {
                _objName = "";

            }
            return _objName;
        }
        set => _objName = value;
    }
    public string ActName
    {
        get => _actName; set => _actName = value;
    }

}
