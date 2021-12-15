using System.Text;
using UnityEngine;

/// <summary>
/// A solution for managing many inventory slots with group behaviour (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class Inventory
{
	public InventorySlot[] Slots { get; private set; }

	public Inventory(int size)
	{
		Slots = new InventorySlot[size];

		for(int i = 0; i < Slots.Length; i++)
		{
			Slots[i] = new InventorySlot();
		}
	}

	/// <summary>
	/// Add an amount of an item to the inventory
	/// </summary>
	/// <returns>The amount that was successfully added</returns>
	public int AddItem(WorldItem item)
	{
		int reserve = item.amount;

		foreach (var slot in Slots)
		{
			if(slot.Item == item.item)
			{
				reserve -= slot.Add(reserve);

				if(reserve == 0)
				{
					return item.amount;
				}
			}
		}

		foreach (var slot in Slots)
		{
			if(slot.Item == null)
			{
				slot.Initialize(item.item, 0);
				reserve -= slot.Add(reserve);

				if(reserve == 0)
				{
					return item.amount;
				}
			}
		}

		return item.amount - reserve;
	}

	/// <summary>
	/// Get the total number of an item in the inventory
	/// </summary>
	/// <returns>The total number of the specified item in the inventory</returns>
	public int GetItemTotal(ItemInfo item)
	{
		int total = 0;

		foreach (var slot in Slots)
		{
			if(slot.Item == item)
			{
				total += slot.Count;
			}
		}

		return total;
	}

	public int SpaceLeftForItem (WorldItem item)
    {
		int spaceLeft = 0;
		foreach (var slot in Slots)
		{
			if (slot.Item == null)
			{
				spaceLeft += item.item.stackSize;
			} else if (slot.Item == item.item)
            {
				spaceLeft += item.item.stackSize - slot.Count;
            }
		}
		return spaceLeft;
	}

	public bool HasEmptySlots ()
    {
		foreach (var slot in Slots)
		{
			if (slot.Item == null)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Remove an amount of an item from the inventory. If there are left overs, they are ignored, so an amount you know is present via GetItemTotal.
	/// </summary>
	public void RemoveItem(WorldItem item)
	{
		foreach (var slot in Slots)
		{
			if(slot.Item == item.item)
			{
				item.amount -= slot.Remove(item.amount);

				if(item.amount == 0)
				{
					return;
				}
			}
		}

		Debug.LogError($"Left over items in RemoveItem(item, amount)! {item.amount}");
	}

	/// <summary>
	/// Remove an amount of an item from the inventory. amountTaken returns the initial item count - amount left
	/// </summary>
	public void RemoveItem(WorldItem item, out int amountTaken)
	{
		int initAmount = item.amount;
		foreach (var slot in Slots)
		{
			if (slot.Item == item.item)
			{
				item.amount -= slot.Remove(item.amount);

				if (item.amount == 0)
				{
					amountTaken = initAmount;
					return;
				}
			}
		}
		amountTaken = initAmount - item.amount;
	}

	/// <summary>
	/// Directly set a slot to an item. Returns an error if the value is out of bounds
	/// </summary>
	public void SetSlot (WorldItem item, int slotIndex)
    {
		if (slotIndex >= Slots.Length)
        {
			Debug.LogError($"SetSlot's slotIndex was outside the allowed bounds! {slotIndex}");
			return;
        }
		Slots[slotIndex].Initialize(item.item, item.amount);
    }
}
