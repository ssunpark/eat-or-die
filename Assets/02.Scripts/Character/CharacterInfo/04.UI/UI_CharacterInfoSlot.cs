using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_CharacterInfoSlot : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private TextMeshProUGUI _nameText;
    
    public void Initialize(int index, CharacterInfoDTO characterInfo)
    {
        _index = index;
        _nameText.text = characterInfo.Name;
    }

    public void OnClickCharacterSlot()
    {
        CharacterInfoManager.Instance.SelectCharacter(_index);
    }
}
