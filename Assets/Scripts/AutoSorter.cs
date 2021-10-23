using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSorter : MonoBehaviour, IItemSaveable, IGetTriggerInfo {

	[SerializeField] int saveID;
	public TellParent tellParent;

	public Transform exit;

	public ItemInfo sortingItem;

	public bool blackListMode;

	public Renderer iconRenderer;

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
		sortingItem = item;
		iconRenderer.gameObject.SetActive(true);
		iconRenderer.material.mainTexture = item.icon.texture;
	}

	public void RemoveItem() {
		sortingItem = null;
		iconRenderer.material.mainTexture = null;
		iconRenderer.gameObject.SetActive(false);
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

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
			SetItem(SaveManager.Instance.FindItem(data.itemID));
		}
	}
}
