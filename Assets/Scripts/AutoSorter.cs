using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSorter : MonoBehaviour, IItemSaveable, IGetTriggerInfo, IItemInteractable {

	[SerializeField] ItemHandler handler;
	[SerializeField] string saveID;
	public TellParent tellParent;

	public Transform exit;

	public ItemInfo sortingItem;

	public bool blackListMode;

	public Renderer iconRenderer;

	public void Interact (WorldItem item)
    {
		SetItem(item.item);
    }

	public void GetTriggerInfo (Collider col)
    {
		if (col.CompareTag("Item"))
		{
			TrySortItem(col.GetComponentInParent<ItemHandler>());
		}
	}

	public void GetTriggerInfoRepeating (Collider col)
    {
		if (col.CompareTag("Item"))
		{
			TrySortItem(col.GetComponentInParent<ItemHandler>());
		}
	}

	void TrySortItem (ItemHandler handler)
    {
		if (handler)
		{
			if (handler.item == sortingItem && !blackListMode || handler.item != sortingItem && blackListMode)
			{
				handler.gameObject.transform.position = exit.position;
				Rigidbody objRB = handler.GetComponent<Rigidbody>();
				if (objRB)
				{
					objRB.velocity = transform.forward * 2f;
				}
			}
		}
	}

	public void SetItem(ItemInfo item) {
		if (sortingItem == item)
        {
			SetBlacklistMode(!blackListMode);
			return;
        }
		if (!item)
		{
			RemoveItem();
			return;
		}
		sortingItem = item;
		iconRenderer.gameObject.SetActive(true);
		iconRenderer.material.mainTexture = item.icon.texture;
	}

	public void RemoveItem() {
		sortingItem = null;
		iconRenderer.material.mainTexture = null;
		iconRenderer.gameObject.SetActive(false);
	}

	void SetBlacklistMode (bool mode)
    {
		blackListMode = mode;
    }

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));

		newData.boolVal = blackListMode;

		if (sortingItem)
		{
			newData.itemID = sortingItem.id;
		}
		else
		{
			newData.itemID = -1;
		}

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		if (data.itemID != -1)
		{
			SetItem(ItemDatabase.Instance.GetItem(data.itemID));
		}
		SetBlacklistMode(data.boolVal);
	}
}
