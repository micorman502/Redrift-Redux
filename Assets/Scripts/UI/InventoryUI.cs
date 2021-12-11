using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A UI representation of an Inventory (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class InventoryUI : MonoBehaviour
{
	
	[SerializeField] internal Transform container;
	[SerializeField] internal GameObject slotPrefab;

	public virtual void Assign(Inventory inventory)
	{
		for (int i = 0; i < container.childCount; i++)
        {
			Destroy(container.GetChild(i).gameObject);
        }
		foreach (var slot in inventory.Slots)
		{
			Instantiate(slotPrefab, container).GetComponent<InventorySlotUI>().Initialize(slot);
		}
	}
}
