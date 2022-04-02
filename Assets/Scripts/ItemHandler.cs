using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour, IItemSaveable {

	[SerializeField] HotTextHandler handler;
	public ItemInfo item;

	[SerializeField] string saveID;
	[SerializeField] bool dontSave;
	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));


		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	void Start ()
    {
		handler.AddHotText(new HotTextInfo("to pickup <" + item.itemName + ">", KeyCode.E, 7, "itemHandlerPickup"));
    }

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;
	}
}