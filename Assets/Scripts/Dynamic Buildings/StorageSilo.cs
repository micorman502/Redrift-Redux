using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageSilo : MonoBehaviour, IItemPickup, IItemSaveable, IItemInteractable, IGetTriggerInfo, IHotText
{
	ExpandedInventorySlot slot;
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
		if (slot != null)
			return;

		slot = new ExpandedInventorySlot();
		slot.MaxStackMultiplier = stackSizeMultiplier;
	}

	public WorldItem[] GetItems ()
	{
		WorldItem[] pickups = new WorldItem[1];

		pickups[0] = new WorldItem(slot.Item, slot.Count);

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
			if (AddItem(itemHandler.item))
			{
				itemHandler.gameObject.SetActive(false);
				Destroy(itemHandler.gameObject);
			}
		}
	}

	bool AddItem (ItemInfo item)
	{
		if (item == null)
			return false;

		if (!slot.Item)
		{
			slot.Initialize(item, 1);
			return true;
		}
		if (slot.Item == item)
		{
			slot.Add(1);
			return true;
		}

		return false;
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
		if (!AddItem(item.item))
			return;

		PlayerInventory inventory = PlayerController.currentPlayer.gameObject.GetComponent<PlayerInventory>();
		inventory.inventory.RemoveItem(new WorldItem(item.item, 1));
	}

	public void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

		newData.num = slot.Count;
		newData.itemID = slot.Item ? slot.Item.id : -1;

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
			slot.Initialize(ItemDatabase.GetItem(data.itemID), data.num);
		}

		UpdateVisuals();
	}

	void UpdateVisuals ()
	{
		if (!slot.Item)
		{
			itemText.text = "";
			return;
		}

		itemText.text = $"{slot.Item.itemName} {slot.Item.stackPrefix}{slot.Count}";

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
		//string infoText = (sortingItem ? sortingItem.name : "") + (blackListEnabled ? " (Blacklist)" : " (Whitelist)");
		HotTextManager.Instance.UpdateHotText(new HotTextInfo("Add Item", KeyCode.F, HotTextInfo.Priority.Interact, "storageSiloInteract"));
	}
}
