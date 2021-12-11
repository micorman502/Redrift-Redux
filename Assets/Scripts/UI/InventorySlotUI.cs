using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A UI representation of an InventorySlot (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
	[SerializeField] Image iconImage;
	[SerializeField] TMP_Text countText;

	protected InventorySlot slot;

	public void Initialize(InventorySlot slot)
	{
		this.slot = slot;

		slot.CountChanged += OnCountChanged;
		slot.ItemChanged += OnItemChanged;

		OnItemChanged(slot.Item);
		OnCountChanged(slot.Count);
	}

	private void OnDestroy()
	{
		slot.CountChanged -= OnCountChanged;
		slot.ItemChanged -= OnItemChanged;
	}

	void OnItemChanged(ItemInfo item)
	{
		if(item == null)
		{
			iconImage.sprite = null;
			iconImage.color = Color.clear;
		}
		else
		{
			iconImage.sprite = item.icon;
			iconImage.color = Color.white;
		}
	}

	void OnCountChanged(int count)
	{
		if (count <= 1)
		{
			countText.text = "";
		}
		else
		{
			countText.text = count.ToString();
		}
	}
}
