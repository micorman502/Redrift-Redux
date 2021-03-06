using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour, IItemPickup, IItemSaveable, IItemInteractable, IGetTriggerInfo {

	[SerializeField] string saveID;

	public GameObject fireLight;
	public ParticleSystem smoke;
	public ParticleSystem fire;

	[HideInInspector] public float fuel = 0;
	[HideInInspector] public OreInfo currentSmeltingItem;
	[SerializeField] Transform dropPos;

	public float smeltTime = 10f;

	public WorldItem[] Pickup ()
	{
		List<WorldItem> pickups = new List<WorldItem>();

		if (currentSmeltingItem) {
			pickups.Add(new WorldItem(currentSmeltingItem, 1));
		}

		return pickups.ToArray();
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
				Destroy(itemHandler.gameObject);
			}
		}
	}

	bool AddItem (ItemInfo item)
    {
		if (item is FuelInfo)
        {
			fuel += (item as FuelInfo).fuelValue;
			return true;
        }

		if (item is OreInfo && !currentSmeltingItem)
        {
			if (fuel > 0)
			{
				StartSmelting(item);
				return true;
			}
        }

		return false;
    }

	public void GetTriggerInfoRepeating (Collider col)
    {
		GetTriggerInfo(col);
    }

	void Check ()
    {
		if (currentSmeltingItem && fuel >= 1)
        {
			StartSmelting(currentSmeltingItem);
        }
    }

	void StartSmelting(ItemInfo item) {
		if (fuel <= 0)
			return;

		currentSmeltingItem = item as OreInfo;
		smoke.Play();
		fire.Play();
		fireLight.SetActive(true);
		Invoke("FinishSmelting", smeltTime);
	}

	void FinishSmelting ()
    {
		DropItem(currentSmeltingItem.smeltResult);
		fuel--;
		StopSmelting();
	}

	void StopSmelting() {
		currentSmeltingItem = null;
		smoke.Stop();
		fire.Stop();
		fireLight.SetActive(false);
	}

	void DropItem(ItemInfo item) {
		GameObject smeltedItemObj = Instantiate(item.droppedPrefab, dropPos ? dropPos.position : transform.position + transform.forward * 0.25f - transform.up * 0.75f, item.droppedPrefab.transform.rotation) as GameObject;
		Rigidbody objRB = smeltedItemObj.GetComponent<Rigidbody>();
		if(objRB) {
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

	public void GetData (out ItemSaveData data,out ObjectSaveData objData, out bool dontSave)
    {
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));

		newData.floatVal = fuel;
		if (currentSmeltingItem)
		{
			newData.itemID = currentSmeltingItem.id;
		}
		else
		{
			newData.itemID = -1;
		}

		data = newData;
		objData = newObjData;
		dontSave = false;
    }

	public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
		transform.position = objData.position;
		transform.rotation = objData.rotation;
		fuel = data.floatVal;
		if (data.itemID != -1)
		{
			currentSmeltingItem = ItemDatabase.Instance.GetItem(data.itemID) as OreInfo;
		}
		Check();
	}
}