using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour, IItemSaveable {
	[SerializeField] string saveID;
	public GameObject applePrefab;
	public Transform[] appleSpawnLocations;

	public float appleSpawnChance = 0.8f;

	[SerializeField] ResourceHandler handler;

	[HideInInspector] public List<GameObject> apples = new List<GameObject>();

	[HideInInspector] public bool spawnApples = true;

	void Start() {
		if(spawnApples) {
			foreach(Transform spawn in appleSpawnLocations) {
				System.Random prng = new System.Random(Mathf.RoundToInt(gameObject.transform.position.sqrMagnitude * 2));
				if(prng.Next(100) / 100f  < appleSpawnChance) {
					GameObject appleObj = Instantiate(applePrefab, spawn.position, spawn.rotation);
					Rigidbody appleRB = appleObj.GetComponent<Rigidbody>();
					apples.Add(appleObj);
					if(appleRB) {
						Destroy(appleRB);
					}
				}
			}
		}
	}

	public void DropFruits() {
		foreach(GameObject apple in apples) {
			if(apple) {
				Instantiate(applePrefab, apple.transform.position, apple.transform.rotation);
				Destroy(apple);
			}
		}
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));

		newData.num = handler.health;
		newData.boolVal = true;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		spawnApples = data.boolVal;

		handler.health = data.num;

		Debug.Log("loaded");
	}
}
