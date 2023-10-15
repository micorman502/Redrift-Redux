using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IItemSaveable, IGetTriggerInfo, IInteractable {

	[SerializeField] ItemHandler handler;
	[SerializeField] string saveID;
	public float[] speeds = { 0f, 1f, 2f, 8f };
	public int speedNum = 0;

	bool active = false;

	Animator anim;

	public TellParent tellParent;
	AudioSource audioSource;

	void Awake() {
		anim = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		UpdateActive();
		UpdateSpeed();
	}

	public void GetTriggerInfoRepeating (Collider col)
	{
		if (!active)
			return;

		Rigidbody itemRB = col.attachedRigidbody;
		if (!itemRB)
			return;

		itemRB.velocity = (itemRB.velocity + transform.forward).normalized * speeds[speedNum];
		//itemRB.AddForce((transform.forward * speeds[speedNum]) - Vector3.Project(itemRB.velocity - Vector3.one, transform.forward.normalized));
		//itemRB.AddRelativeForce(transform.forward * speeds[speedNum] - itemRB.velocity); // TODO: PUT IN FIXEDUPDATE!!! //Waltuh
	}

	public void GetTriggerInfo (Collider col)
    {
		//dummy
    }

	public void Interact ()
    {
		IncreaseSpeed();
    }

	public void SetSpeed(int speed) {
		speedNum = speed;
		if(speedNum >= speeds.Length) {
			speedNum = 0;
		}
		if(speeds[speedNum] == 0) {
			active = false;
			UpdateActive();
		} else if(!active) {
			active = true;
			UpdateActive();
		}
		UpdateSpeed();
	}

	public void IncreaseSpeed() {
		SetSpeed(speedNum + 1);
	}

	public void ToggleActive() {
		active = !active;
		UpdateActive();
	}

	void UpdateActive() {
		anim.SetBool("Active", active);
		audioSource.mute = !active;
	}

	void UpdateSpeed() {
		anim.SetFloat("Speed", speeds[speedNum]);
		audioSource.pitch = 0.5f + speeds[speedNum] / 8f;
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

		newData.num = speedNum;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		SetSpeed(data.num);
	}
}
