using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Hand : MonoBehaviour
{
	public Image IconImage;
	public TextMeshProUGUI QuantityText;

	private void Awake()
	{
		IconImage.gameObject.SetActive(false);
		QuantityText.gameObject.SetActive(false);
	}
	
	private void Start()
	{
		HandEntity.Instance.OnItemPickedUp += UpdateHandUI;
		UpdateHandUI();
	}

	private void Update()
	{
		// 캔버스 위에 존재하는 UI_Hand는 마우스 포지션을 따라다님
		Vector2 mousePosition = Input.mousePosition;
		transform.position = mousePosition;
		
	}
	
	public void UpdateHandUI()
	{
		ItemStack itemInHand = HandEntity.Instance.ItemStack;
		if (itemInHand == null)
		{
			IconImage.gameObject.SetActive(false);
			QuantityText.gameObject.SetActive(false);
			return;
		}
		
		IconImage.sprite = ItemManager.Instance.GetItem(itemInHand.ID).ItemData.Icon;
		QuantityText.text = itemInHand.Quantity.ToString();
		IconImage.gameObject.SetActive(true);
		QuantityText.gameObject.SetActive(itemInHand.Quantity > 1);
	}
	
}
