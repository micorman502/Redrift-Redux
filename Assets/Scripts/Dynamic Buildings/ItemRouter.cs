using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRouter : MonoBehaviour, IItemSaveable, IGetTriggerInfo
{
	[SerializeField] string saveID;
    [SerializeField] Transform[] outputPoints;
    int currentOutput;

	public void GetTriggerInfo (Collider col)
	{
		if (col.CompareTag("Item"))
		{
			TryRouteItem(col.GetComponentInParent<ItemHandler>());
		}
	}

	public void GetTriggerInfoRepeating (Collider col)
	{
		if (col.CompareTag("Item"))
		{
			TryRouteItem(col.GetComponentInParent<ItemHandler>());
		}
	}

	void TryRouteItem (ItemHandler handler)
	{
		if (!handler)
			return;

		handler.gameObject.transform.position = outputPoints[currentOutput].position;
		Rigidbody objRB = handler.GetComponent<Rigidbody>();
		if (objRB)
		{
			objRB.velocity = transform.forward * 2f;
		}

		currentOutput++;

		if (currentOutput >= outputPoints.Length)
        {
			currentOutput = 0;
        }
	}

	public virtual void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

		newData.num = currentOutput;

		data = newData;
		objData = newObjData;
		_dontSave = false;
	}

	public void SetData (ItemSaveData data, ObjectSaveData objData)
	{
		Load(data, objData);
	}

	protected virtual void Load (ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		currentOutput = data.num;
	}
}
