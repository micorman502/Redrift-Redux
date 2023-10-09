using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour, IItemSaveable, IItemPickup, IHotText {

	public ItemInfo item;

	[SerializeField] string saveID;
	[SerializeField] bool dontSave;
	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		if (dontSave)
        {
			data = new ItemSaveData();
			objData = new ObjectSaveData();
			_dontSave = dontSave;
			return;
		}

		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));


		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;
	}

	public WorldItem[] GetItems ()
    {
		return new WorldItem[] { new WorldItem(item, 1) };
	}

	public void Pickup ()
    {
		Destroy(gameObject);
	}

	void IHotText.HideHotText()
    {
		HotTextManager.Instance.RemoveHotText(new HotTextInfo("Pickup", KeyCode.E, HotTextInfo.Priority.Pickup, "itemHandler"));
    }

	void IHotText.ShowHotText()
    {
		HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Pickup", KeyCode.E, HotTextInfo.Priority.Pickup, "itemHandler"));
	}

	void IHotText.UpdateHotText()
    {
		HotTextManager.Instance.UpdateHotText(new HotTextInfo("Pickup", KeyCode.E, HotTextInfo.Priority.Pickup, "itemHandler"));
	}
}