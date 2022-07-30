using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable, IItemSaveable {

	[SerializeField] ItemHandler handler;
	[SerializeField] string saveID;
	Animator anim;
	bool open = false;

	void Start() {
		anim = GetComponent<Animator>();
	}

	public void Interact ()
    {
		ToggleOpen();
    }

	public void ToggleOpen() {
		SetState(!open);
	}

	public void SetState(bool state)
    {
		open = state;

		anim.SetBool("Open", state);
	}

	public void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));

		newData.boolVal = open;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData (ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		SetState(data.boolVal);
	}
}
