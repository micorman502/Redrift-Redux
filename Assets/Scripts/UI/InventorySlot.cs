using System;
using UnityEngine;

/// <summary>
/// Contains a count of an item. Can be added to or removed from, and intereact with other inventory slots. (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class InventorySlot
{
	public ItemInfo Item { get; private set; }
	public int Count { get; private set; }

	public Action<int> CountChanged;
	public Action<ItemInfo> ItemChanged;

	public InventorySlot()
	{
		Clear();
	}

	public void Initialize(ItemInfo item, int count)
	{
		Item = item;
		Count = count;

		ItemChanged?.Invoke(item);
		CountChanged?.Invoke(Count);
	}

	/// <summary>
	/// Add an amount to this inventory slot
	/// </summary>
	/// <returns>The amount added</returns>
	public int Add(int amount)
	{
		// the total amount we could possibly add to this slot
		int possible = Item.stackSize - Count;

		if(amount > possible)
		{
			amount = possible;
		}

		Count += amount;
		if(Count == 0)
		{
			Clear();
		}
		CountChanged?.Invoke(Count);

		return amount;
	}

	/// <summary>
	/// Remove an amount from this inventory slot
	/// </summary>
	/// <returns>The amount removed</returns>
	public int Remove(int amount)
	{
		if(amount > Count)
		{
			amount = Count;
		}

		Count -= amount;
		if(Count == 0)
		{
			Clear();
		}
		CountChanged?.Invoke(Count);

		return amount;
	}

	/// <summary>
	/// Dump this slot's contents on another slot. This slot will dump as much as possible, and retain the rest.
	/// A common situation in most games would be when you drag this slot onto another one, and it moves or adds to the other slot's contents.
	/// </summary>
	public void Dump(InventorySlot victim)
	{
		if(Item == victim.Item)
		{
			Count -= victim.Add(Count);
			if(Count == 0)
			{
				Clear();
			}
			CountChanged?.Invoke(Count);
		}
		else
		{
			Swap(victim);
		}
	}

	/// <summary>
	/// Swap the contents of this and another slot
	/// </summary>
	public void Swap(InventorySlot other)
	{
		ItemInfo tempItem = other.Item;
		int tempCount = other.Count;

		other.Initialize(Item, Count);
		Initialize(tempItem, tempCount);
	}

	public void Split(InventorySlot other)
	{
		if(Item == other.Item || other.Item == null) // cant do shit if its not the same item
		{
			if(other.Item == null)
			{
				other.Initialize(Item, 0);
			}

			Count -= other.Add(Mathf.CeilToInt((float)Count / 2));
			if(Count == 0)
			{
				Clear();
			}
			CountChanged?.Invoke(Count);
		}
	}

	public void Clear()
	{
		Initialize(null, 0);
	}

	public override string ToString()
	{
		return $"{{ {(Item ? Item.ToString() : "Null")}, {Count.ToString()} }}";
	}
}

/*
using System;

public class InventorySlot
{
	// I use the _ to mean NEVER modify this outside of its getter/setter method
	Item _item;
	public Item Item
	{
		get
		{
			return _item;
		}
		set
		{
			_item = value;

			ItemChanged?.Invoke(_item);
			if(_item == null)
			{
				_count = 0;
			}
		}
	}

	int _count;
	public int Count
	{
		get
		{
			return _count;
		}
		set
		{
			_count = value;

			CountChanged?.Invoke(_count);
			if(_count == 0)
			{
				_item = null;
			}
		}
	}

	public Action<int> CountChanged;
	public Action<Item> ItemChanged;

	public InventorySlot()
	{
		Clear();
	}

	public void SetItem(Item item)
	{
		Item = item;
	}

	/// <summary>
	/// Add an amount to this inventory slot
	/// </summary>
	/// <returns>The amount added</returns>
	public int Add(int amount)
	{
		// the total amount we could possibly add to this slot
		int possible = Item.stackSize - Count;

		if(amount > possible)
		{
			amount = possible;
		}

		Count += amount;

		return amount;
	}

	/// <summary>
	/// Remove an amount from this inventory slot
	/// </summary>
	/// <returns>The amount removed</returns>
	public int Remove(int amount)
	{
		if(amount > Count)
		{
			amount = Count;
		}

		Count -= amount;

		return amount;
	}

	/// <summary>
	/// Dump this slot's contents on another slot. This slot will dump as much as possible, and retain the rest.
	/// A common situation in most games would be when you drag this slot onto another one, and it moves or adds to the other slot's contents.
	/// </summary>
	public void Dump(InventorySlot victim)
	{
		if(Item == victim.Item)
		{
			Count -= victim.Add(Count);
		}
		else
		{
			Swap(victim);
		}
	}

	/// <summary>
	/// Swap the contents of this and another slot
	/// </summary>
	public void Swap(InventorySlot other)
	{
		Item tempItem = other.Item;
		int tempCount = other.Count;

		other.Initialize(Item, Count);
		Initialize(tempItem, tempCount);
	}

	public void Clear()
	{
		Initialize(null, 0);
	}

	public override string ToString()
	{
		return $"{{ {(Item ? Item.ToString() : "Null")}, {Count.ToString()} }}";
	}
}

*/
