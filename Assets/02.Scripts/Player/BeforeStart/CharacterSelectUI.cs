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
        int selectedClassIndex = ClassSelector.CurrentIndex;

        CustomizationData data = new CustomizationData();
        data.Top = (short)Customizer.GetActualIndex("Top");
        data.Bottom = (short)Customizer.GetActualIndex("Bottom");
        data.Hair = (short)Customizer.GetActualIndex("Hair");
        data.Eye = (short)Customizer.GetActualIndex("Eye");
        CustomizationDataHolder.Instance.CustomizationData = data;
        CustomizationDataHolder.Instance.Nickname = NicknameInput.text;
        CustomizationDataHolder.Instance.ClassType = (ECharacterType)selectedClassIndex;

        SceneManager.LoadScene("LoadingLoadingTestScene");
    }
}
