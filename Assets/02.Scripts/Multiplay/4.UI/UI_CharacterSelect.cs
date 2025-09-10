using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_CharacterSelect : AUI_PopupBase
{
    public override EPopupType Type => EPopupType.World;
    
    [SerializeField] private GameObject _container; 
    [SerializeField] private UI_CharacterInfoSlot _chracterInfoSlotPrefab;
    
    private List<GameObject> _characterInfoSlotList = new ();

    public void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        Debug.Log("CharacterSlot Refresh");
        DestroyAllSlots();
        
        List<CharacterInfoDTO> loadedCharacterList = CharacterInfoManager.Instance.CharacterInfoDTOList;

        if (loadedCharacterList == null)
        {
            Debug.Log("CharacterInfoManager.Instance.CharacterInfoDTOList is null");
            return;
        }
        
        for (int i = 0; i < loadedCharacterList.Count; ++i)
        {
            UI_CharacterInfoSlot slot = Instantiate(_chracterInfoSlotPrefab, _container.transform);
            slot.Initialize(i, loadedCharacterList[i]);
            slot.transform.SetSiblingIndex(i);
            slot.gameObject.SetActive(true);
            _characterInfoSlotList.Add(slot.gameObject);
        }
    }

    private void DestroyAllSlots()
    {
        for (int i = _characterInfoSlotList.Count - 1; i >= 0; --i)
        {
            Destroy(_characterInfoSlotList[i]);
        }
    }
}
