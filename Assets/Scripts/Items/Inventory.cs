using System.Text;
using System;
using UnityEngine;

/// <summary>
/// A solution for managing many inventory slots with group behaviour (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class Inventory
{
	public Action InventoryChanged; // Only triggers when the inventory is changed via any of the functions it provides.
	public Action<WorldItem> ItemOverflow; 
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
	public int AddItem (WorldItem item)
    {
		return AddItem(item.item, item.amount);
    }
	public int AddItem(ItemInfo item, int amount)
	{
		int reserve = amount;

		foreach (var slot in Slots)
		{
			if(slot.Item == item)
			{
				reserve -= slot.Add(reserve);

				if(reserve == 0)
				{
					InventoryChanged?.Invoke();
					return amount;
				}
			}
		}

		foreach (var slot in Slots)
		{
			if(slot.Item == null)
			{
				slot.Initialize(item, 0);
				reserve -= slot.Add(reserve);

				if(reserve == 0)
				{
					InventoryChanged?.Invoke();
					return amount;
				}
			}
		}

		InventoryChanged?.Invoke();
		if (reserve > 0)
        {
			ItemOverflow?.Invoke(new WorldItem(item,  reserve));
        }
		return amount;
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
	/// <summary>
	/// 
	/// </summary>
	/// <returns>How much of a certain item can fit within this inventory.</returns>
	public int SpaceLeftForItem (WorldItem item)
    {
		return SpaceLeftForItem(item.item, item.amount);
    }
	public int SpaceLeftForItem (ItemInfo item, int amount)
    {
		int spaceLeft = 0;
		foreach (var slot in Slots)
		{
			if (slot.Item == null)
			{
				spaceLeft += item.stackSize;
			} else if (slot.Item == item)
            {
				spaceLeft += item.stackSize - slot.Count;
            }
		}
		return spaceLeft;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="includeIncompleteStacks">If true, any InventorySlot with a Count above 0 counts towards the inventory being full</param>
	/// <returns></returns>
	public bool InventoryFull (bool includeIncompleteStacks)
    {
		foreach (InventorySlot slot in Slots)
        {
			if (slot.Count <= 0)
            {
				return false;
            }
			if (slot.Count < slot.Item.stackSize && !includeIncompleteStacks)
            {
				return false;
            }
        }

		return true;
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
	/// <returns>The amount of items that were successfully taken.</returns>
	public int RemoveItem (WorldItem item)
    {
		return RemoveItem(item.item, item.amount);
    }

	public int RemoveItem(ItemInfo item, int amount)
	{
		int amountTaken = 0;
		foreach (var slot in Slots)
		{
			if(slot.Item == item)
			{
				int removingAmt = slot.Remove(amount);
				amount -= removingAmt;
				amountTaken += removingAmt;

				if(amount == 0)
				{
					InventoryChanged?.Invoke();
					return amountTaken;
				}
			}
		}

		return amountTaken;
	}

	public int RemoveItem (ItemInfo item)
    {
		return RemoveItem(item, 1);
    }

	/// <summary>
	/// Directly set a slot to an item. Returns an error if the value is out of bounds
	/// </summary>
	public void SetSlot (WorldItem item, int slotIndex)
    {
		SetSlot(item.item, item.amount, slotIndex);
    }
	public void SetSlot (ItemInfo item, int amount, int slotIndex)
    {
		if (slotIndex >= Slots.Length)
        {
			Debug.LogError($"SetSlot's slotIndex was outside the allowed bounds at slotIndex: {slotIndex}");
			return;
        }
		Slots[slotIndex].Initialize(item, amount);
		InventoryChanged?.Invoke();
	}
}
