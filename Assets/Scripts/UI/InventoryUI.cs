using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A UI representation of an Inventory (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class InventoryUI : MonoBehaviour
{
	[SerializeField] Transform container;
	[SerializeField] GameObject slotPrefab;

	public void Initialize(Inventory inventory)
	{
		foreach (var slot in inventory.Slots)
		{
			Instantiate(slotPrefab, container).GetComponent<InventorySlotUI>().Initialize(slot);
		}
	}
}
