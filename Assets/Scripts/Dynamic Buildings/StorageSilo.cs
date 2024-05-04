using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageSilo : MonoBehaviour, IItemPickup, IItemSaveable, IItemInteractable, IGetTriggerInfo, IHotText
{
	Inventory inventory;
	[SerializeField] string saveID;
	[SerializeField] int stackSizeMultiplier = 2;

	[SerializeField] Transform dropPos;
	[SerializeField] TMP_Text itemText;

	void Awake ()
	{
		Initialise();
	}

	void Initialise ()
	{
		if (inventory != null)
			return;

		inventory = Inventory.CreateCustomInventory<ExpandedInventorySlot>(1);
		ExpandedInventorySlot expandedSlot = inventory.Slots[0] as ExpandedInventorySlot;

		expandedSlot.MaxStackMultiplier = stackSizeMultiplier;

		UpdateVisuals();
	}

	public WorldItem[] GetItems ()
	{
		WorldItem[] pickups;

		if (inventory.Slots[0].Item)
        {
			pickups = new WorldItem[1];
			pickups[0] = new WorldItem(inventory.Slots[0].Item, inventory.Slots[0].Count);
		} else
        {
			pickups = new WorldItem[0];
        }

		return pickups;
	}

	public void Pickup ()
	{
		//dummy
	}

	public void GetTriggerInfo (Collider col)
	{
		if (col.CompareTag("Item"))
		{
			ItemHandler itemHandler = col.GetComponent<ItemHandler>();
			if (!itemHandler)
				return;
			if (inventory.AddItem(itemHandler.item, 1) > 0)
			{
				UpdateVisuals();

				itemHandler.gameObject.SetActive(false);
				Destroy(itemHandler.gameObject);
			}
		}
	}

	public void GetTriggerInfoRepeating (Collider col)
	{
		GetTriggerInfo(col);
	}

	void DropItem (ItemInfo item)
	{
		GameObject smeltedItemObj = Instantiate(item.droppedPrefab, dropPos ? dropPos.position : transform.position + transform.forward * 0.25f - transform.up * 0.75f, item.droppedPrefab.transform.rotation) as GameObject;
		Rigidbody objRB = smeltedItemObj.GetComponent<Rigidbody>();
		if (objRB)
		{
			objRB.velocity = transform.forward;
		}
	}

	public void Interact (WorldItem item)
	{
		Inventory playerInventory = PlayerController.currentPlayer.gameObject.GetComponent<PlayerInventory>().inventory;

		bool addingItem = (inventory.HasEmptySlots() == true || inventory.Slots[0].Item == item.item) && item.item != null;
		
		if (addingItem)
        {
			inventory.AddItem(item.item, 1);
			playerInventory.RemoveItem(item.item, 1);
			UpdateVisuals();
			return;
        }

		if (inventory.HasEmptySlots())
			return;

		playerInventory.AddItem(inventory.Slots[0].Item, 1, true);
		inventory.RemoveItem(inventory.Slots[0].Item, 1);

		UpdateVisuals();
	}

	public void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

		newData.num = inventory.Slots[0].Count;
		newData.itemID = inventory.Slots[0].Item ? inventory.Slots[0].Item.id : -1;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData (ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		Initialise();

		if (data.itemID > -1)
		{
			inventory.SetSlot(ItemDatabase.GetItem(data.itemID), data.num, 0);
		}

		UpdateVisuals();
	}

	void UpdateVisuals ()
	{
		if (!inventory.Slots[0].Item)
		{
			itemText.text = GetCapacityInfo(4);
			return;
		}

		itemText.text = $"{inventory.Slots[0].Item.itemName}\n{inventory.Slots[0].Item.stackPrefix}{inventory.Slots[0].Count}\n{GetCapacityInfo(4)}";

	}

	string GetCapacityInfo (int length)
    {
		float capacityUsed = 0;

		capacityUsed = inventory.Slots[0].MaxStack == 0 ? 0 : inventory.Slots[0].Count / (float)inventory.Slots[0].MaxStack;

		string capacityInfo = "[";
		for (int i = 1; i < length + 1; i++)
        {
			if (capacityUsed >= (1f / length) * i)
            {
				capacityInfo += "#";
            } else
            {
				capacityInfo += "-";
            }
        }
		capacityInfo += "]";

		return capacityInfo;
	}

	public void HideHotText ()
	{
		HotTextManager.Instance.RemoveHotText("storageSiloInteract");
	}

	public void ShowHotText ()
	{
		//string infoText = (sortingItem ? sortingItem.name : "") + (blackListEnabled ? " (Blacklist)" : " (Whitelist)");
		HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Add Item", KeyCode.F, HotTextInfo.Priority.Interact, "storageSiloInteract"));
	}

	public void UpdateHotText ()
	{
		string infoText = inventory.HasEmptySlots() ? "Add Item" : (inventory.InventoryFull(false) ? "Remove Item" : "Remove/Add Item");
		HotTextManager.Instance.UpdateHotText(new HotTextInfo(infoText, KeyCode.F, HotTextInfo.Priority.Interact, "storageSiloInteract"));
	}
}
