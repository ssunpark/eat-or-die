using UnityEngine;
using UnityEngine.UI;

public class UI_SkillNode : MonoBehaviour
{
    [SerializeField]
    private Sprite _activeSprite;
    [SerializeField]
    private Sprite _inactiveSprite;
    
    private Image _image;
    
    private int _id;

    public void Bind(int id)
    {
        _id = id;
    }

    public void Refresh()
    {
        bool isActive = SkillManager.Instance.IsActive(_id);
        _image.sprite = isActive ? _activeSprite : _inactiveSprite;
    }
}