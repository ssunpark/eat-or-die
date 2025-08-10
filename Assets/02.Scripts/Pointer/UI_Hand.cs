using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Hand : MonoBehaviour
{
	public Image IconImage;
	public TextMeshProUGUI QuantityText;
	
	private void Start()
	{
		HandEntity.Instance.OnItemPickedUp += UpdateHandUI;
		UpdateHandUI();
	}

	private void Update()
	{
		Vector2 mousePosition = Input.mousePosition;
		transform.position = mousePosition;
	}
	
	public void UpdateHandUI()
	{
		ItemInstance itemInstanceInHand = HandEntity.Instance.ItemInstance;
		if (itemInstanceInHand == null)
		{
			IconImage.gameObject.SetActive(false);
			QuantityText.gameObject.SetActive(false);
			return;
		}
		
		IconImage.sprite = itemInstanceInHand.ItemProfile.ItemDefinition.Icon;
		QuantityText.text = itemInstanceInHand.Quantity.ToString();
		IconImage.gameObject.SetActive(true);
		QuantityText.gameObject.SetActive(itemInstanceInHand.Quantity > 1);
	}
	
}
