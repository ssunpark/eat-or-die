using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    public ClassSelector ClassSelector;
    public CharacterCustomizer Customizer;
    public TMP_InputField NicknameInput;

    public void OnStartButtonPressed()
    {
        CharacterInfoDTO newCharacterInfo = new CharacterInfoDTO();
        
        newCharacterInfo.Name = NicknameInput.text;
        newCharacterInfo.IsInit = false;
        newCharacterInfo.Class = ClassSelector.CurrentIndex;
        
        newCharacterInfo.CreatedAt = Timestamp.GetCurrentTimestamp();
        newCharacterInfo.LastLoginAt = Timestamp.GetCurrentTimestamp();
        
        newCharacterInfo.Top = Customizer.GetActualIndex("Top");
        newCharacterInfo.Bottom = Customizer.GetActualIndex("Bottom");
        newCharacterInfo.Hair = Customizer.GetActualIndex("Hair");
        newCharacterInfo.Eye = Customizer.GetActualIndex("Eye");
        
        CharacterInfoManager.Instance.CreateNewCharacter(newCharacterInfo);
        // SceneManager.LoadScene("LoadingScene");
    }
}
