using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlot : MonoBehaviour {
	WorldItem item;
	[SerializeField] Image icon;
	[SerializeField] TMP_Text amountText;

	public void Setup (WorldItem newItem)
	{
		item = newItem;
		icon.sprite = item.item.icon;
		amountText.text = item.amount > 1 ? item.amount.ToString() : "";
	}

	public void OnItemPointerEnter() {
		if(item.item) {
			InventoryEvents.SetHoveredItem(item.item, null);
		}
	}

	public void OnItemPointerExit() {
		InventoryEvents.LeaveHoveredItem();
	}
}
