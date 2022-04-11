using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallIsland : MonoBehaviour, IItemSaveable {

	Rigidbody rb;
	[SerializeField] GameObject destructionParticles;
	[SerializeField] Vector3 bounds;

	public string saveID;

	public Vector3 moveAmount;

	float speed = 0.5f;
	[SerializeField] float minSpeed, maxSpeed;
	bool loaded;
	int curTick;

	void Start ()
    {
		rb = gameObject.GetComponent<Rigidbody>();

		CheckOverlap();

		if (!loaded)
        {
			speed = Random.Range(minSpeed, maxSpeed);
        }
		if (speed == 0)
        {
			Die();
        }
    }

	void FixedUpdate ()
    {
		curTick++;

		rb.MovePosition(transform.position + moveAmount * Time.fixedDeltaTime * speed);

		if (transform.localPosition.z >= 200f)
		{
			Die();
			return;
		}

		if (curTick < 25)
			return;
		else
			curTick = 0;

		CheckOverlap();
 	}

    void CheckOverlap ()
    {
		Collider[] cols = Physics.OverlapBox(transform.position, bounds, Quaternion.identity, LayerMask.GetMask("Small Island", "World"), QueryTriggerInteraction.Ignore);
		if (cols.Length > 1) //more colliders than just this one
		{
			Die();
		}
	}

	void Die ()
    {
		Transform[] children = transform.GetComponentsInChildren<Transform>();

		foreach (Transform child in children)
		{
			if (child.gameObject.GetComponent<PlayerController>())
			{
				child.GetComponent<ArtificialInertia>().SetRootParent(null);
			}
		}

		Destroy(Instantiate(destructionParticles, transform.position, Quaternion.identity), 6f);
		gameObject.SetActive(false);
		Destroy(gameObject);
	}

    public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));

		newData.floatVal = speed;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		speed = data.floatVal;

		loaded = true;
	}
}