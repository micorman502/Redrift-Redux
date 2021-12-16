using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour, IItemSaveable, INoticeText {

	[SerializeField] string tooltip;
	public ItemInfo item;

	[SerializeField] int saveID;
	[SerializeField] bool dontSave;
	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);


		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	public string GetNoticeText ()
    {
		if (tooltip != "")
		{
			return tooltip;
		} else
        {
			return item.itemName;
        }
    }

	public void SetNoticeText (string _tooltip)
    {
		tooltip = _tooltip;
    }

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;
	}

	public void SetSaveID (int newID)
    {
		saveID = newID;
    }
}