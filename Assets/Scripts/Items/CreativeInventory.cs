using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventory : Inventory
{
	public CreativeInventory (int size) : base (size)
	{
		Slots = new CreativeInventorySlot[size];

		for (int i = 0; i < Slots.Length; i++)
		{
			Slots[i] = new CreativeInventorySlot();
		}
	}
}
