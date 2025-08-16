using UnityEngine;
using UnityEngine.UI;

public class UI_HeadPlayerHP : MonoBehaviour
{
    [SerializeField] private Slider _hungerSlider;
    public void InitializeHeadHpBar(ResourceManager resource, StatManager statManager)
    {
        resource.OnHungerChanged += Resource_OnHungerChanged;
        Resource_OnHungerChanged(resource.CurrentHunger, resource.MaxHunger);
    }

    private void Resource_OnHungerChanged(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0)
        {
            Debug.LogWarning("Max health is zero or negative, setting slider value to 0.");
            _hungerSlider.value = 0;
        }
        else
        {
            _hungerSlider.value = currentHealth / maxHealth;
        }
    }


}
