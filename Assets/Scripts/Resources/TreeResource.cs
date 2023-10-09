using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour, IItemSaveable {
	[SerializeField] string saveID;
	public GameObject applePrefab;
	public GameObject droppedApplePrefab;
	public Transform[] appleSpawnLocations;

	public float appleSpawnChance = 0.8f;

	[SerializeField] ResourceHandler handler;

	[HideInInspector] public List<GameObject> apples = new List<GameObject>();

	[HideInInspector] public bool spawnApples = true;

	void Start() {
		if(spawnApples) {
			System.Random prng = new System.Random(Mathf.RoundToInt(gameObject.transform.position.sqrMagnitude * 2));
			float spawnedApples = 0;
			for (int i = 0; i < appleSpawnLocations.Length; i++)
            {
				spawnedApples += prng.Next(101) / 100f < appleSpawnChance ? 1 : 0;
			}

			SpawnApples(Mathf.RoundToInt(spawnedApples));
		}
	}

	void SpawnApples (int amt)
    {
		for (int i = 0; i < appleSpawnLocations.Length && i < amt; i++)
        {
			GameObject appleObj = Instantiate(applePrefab, appleSpawnLocations[i].position, appleSpawnLocations[i].rotation);
			Rigidbody appleRB = appleObj.GetComponent<Rigidbody>();
			apples.Add(appleObj);
			if (appleRB)
			{
				Destroy(appleRB);
			}
		}
    }

	public void DropFruits() {
		foreach(GameObject apple in apples) {
			if(apple) {
				Instantiate(droppedApplePrefab, apple.transform.position, apple.transform.rotation);
				Destroy(apple);
			}
		}
	}

	int GetAppleCount ()
    {
		int ret = 0;
		for (int i = 0; i < apples.Count; i++)
        {
			if (apples[i])
            {
				ret++;
            }
        }
		return ret;
    }

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

		newData.num = handler.health;
		newData.boolVal = true;
		newData.floatVal = GetAppleCount();

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		spawnApples = false;
		SpawnApples(Mathf.RoundToInt(data.floatVal));

		handler.health = data.num;
	}
}
