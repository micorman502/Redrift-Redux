using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	[SerializeField] WorldItem[] items;

	public void SwapItems(InventorySlot slotA, InventorySlot slotB)
	{
		Debug.Log("swapping");
		int first = slotA.GetSlotID();
		int second = slotB.GetSlotID();
		WorldItem itemA = new WorldItem(items[first].item, items[first].amount);
		WorldItem itemB = new WorldItem(items[second].item, items[second].amount);

		items[first] = itemB;
		items[second] = itemA;
		InventoryEvents.UpdateInventorySlot(items[first], first);
		InventoryEvents.UpdateInventorySlot(items[second], second);
	}

	public void SetSlot(WorldItem item, int index)
	{
		items[index] = item;
		InventoryEvents.UpdateInventorySlot(item, index);
	}

	public void AddItem(WorldItem item)
	{
		AddItem(item.item, item.amount);
	}

	public void AddItem(ItemInfo item, int amount)
	{
		if (!item || amount == 0)
			return;
		int amountLeft = amount;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].item == item || items[i].item == null)
			{
				items[i].item = item;
				if (items[i].amount + amountLeft <= items[i].item.stackSize)
				{
					items[i].amount += amountLeft;
					amountLeft = 0;
					InventoryEvents.UpdateInventorySlot(items[i], i);
					break;
				}
				else
				{
					int removed = items[i].item.stackSize - items[i].amount;
					amountLeft -= removed;
					items[i].amount = items[i].item.stackSize;
					InventoryEvents.UpdateInventorySlot(items[i], i);
					if (amountLeft == 0)
					{
						break;
					}
				}
			}
			else
			{// if the item is not the thing we are trying to add here
				continue;
			}
		}

		if (amountLeft > 0)
		{
			Debug.Log("amount left is too much"); //COME BACK
		}

		if (amountLeft != amount)
		{ //if items were actually taken
			AchievementManager.Instance.GetAchievement(item.achievementId);
		}
	}

	/// <summary>
	/// Returns how many items have been taken. Return value of 0 means no items were taken.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="amount"></param>
	/// <returns></returns>
	public int RemoveItem(ItemInfo item, int amount)
	{
		if (!item || amount == 0)
			return 0;
		int amountLeft = 0;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].item == item)
			{
				if (items[i].amount - amountLeft >= 0)
				{
					items[i].amount -= amountLeft;
					amountLeft = amount;
					if (items[i].amount == 0)
					{
						items[i].Clear();
					}
					InventoryEvents.UpdateInventorySlot(items[i], i);
					break;
				}
				else
				{
					int removed = Mathf.Abs(items[i].amount - amountLeft);
					amountLeft += removed;
					items[i].Clear();
					InventoryEvents.UpdateInventorySlot(items[i], i);
					if (amountLeft <= 0)
					{
						break;
					}
				}
			}
			else
			{// if the item is not the thing we are trying to remove here
				continue;
			}
		}

		return amountLeft;
	}

	public int RemoveItem(int index, int amount)
	{
		ItemInfo item = items[index].item;
		return RemoveItem(item, amount);
	}

	public bool RemoveItem(ItemInfo item)
	{
		if (RemoveItem(item, 1) == 0)
		{
			return false; //did not get removed
		}
		else
		{
			return true; //did get removed
		}
	}

	public void SetItem(WorldItem item, int index)
	{
		if (index > -1 && index < items.Length)
		{
			items[index] = item;
			InventoryEvents.UpdateInventorySlot(item, index);
		}
	}

	public void SetItem(ItemInfo item, int amount, int index)
	{
		SetItem(new WorldItem(item, amount), index);
	}

	int Ping(int num, int max)
	{
		if (num > max)
		{
			return max;
		}
		else
		{
			return num;
		}
	}

	public WorldItem[] GetInventory()
	{
		return items;
	}

	public void ClearInventory()
	{
		for (int i = 0; i < items.Length; i++)
		{
			items[i].Clear();
		}

	}
}
