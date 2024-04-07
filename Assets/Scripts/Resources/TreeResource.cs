using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : ResourceHandler {

	[Header("Tree and Apple Info")]
	[SerializeField] GameObject applePrefab;
	[SerializeField] GameObject droppedApplePrefab;
	[SerializeField] Transform[] appleSpawnLocations;

	[SerializeField] float appleSpawnChance = 0.8f;

	[HideInInspector] public List<GameObject> apples = new List<GameObject>();

	[HideInInspector] public bool spawnApples = true;

    protected override void PostLoadInitialise ()
    {
        base.PostLoadInitialise();

		if (spawnApples)
		{
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

    public override void DestroyResource ()
    {
        base.DestroyResource();

		DropFruits();
    }

    public override void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		base.GetData(out ItemSaveData newData, out ObjectSaveData newObjData, out bool _dontSave);

		newData.boolVal = true;
		newData.floatVal = GetAppleCount();

		data = newData;
		objData = newObjData;
		dontSave = _dontSave;
	}

	protected override void Load (ItemSaveData data, ObjectSaveData objData)
	{
		base.Load(data, objData);

		spawnApples = false;
		SpawnApples(Mathf.RoundToInt(data.floatVal));
	}
}
