using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
	[SerializeField] float checkRate;
	int curTick;
	[SerializeField] Transform spawnTarget;
	[SerializeField] Vector3 bounds;
    [SerializeField] ResourceSpawn[] spawns;

	private void Start ()
    {
		if (PersistentData.Instance)
		{
			if (!PersistentData.Instance.loadingFromSave)
			{
				SpawnInitialResources();
			}
		}
		else
		{
			SpawnInitialResources();
		}

		Initialise();
    }

	void Initialise ()
    {
		for (int i = 0; i < spawns.Length; i++)
        {
			spawns[i].SetNextSpawnTime();
        }
    }

	void SpawnInitialResources ()
    {
		for (int i = 0; i < spawns.Length; i++)
		{
			int initialSpawnAmt = Random.Range(spawns[i].initialSpawnAmountMin, spawns[i].initialSpawnAmountMax + 1);

			for (int s = 0; s < initialSpawnAmt; s++)
			{
				TrySpawnResource(spawns[i]);
			}
		}
	}


    void FixedUpdate ()
	{
		curTick++;
		if (curTick < Mathf.RoundToInt(checkRate / Time.fixedDeltaTime))
			return;
		else
			curTick = 0;

		for (int i = 0; i < spawns.Length; i++)
        {
			CheckResourceSpawn(spawns[i]);
        }
	}

	void CheckResourceSpawn (ResourceSpawn spawn)
    {
		if (Time.time < spawn.nextSpawnTime)
			return;

		spawn.SetNextSpawnTime();

		TrySpawnResource(spawn);
    }

	void TrySpawnResource (ResourceSpawn spawn)
    {
		Vector3 checkPos = spawnTarget.position + spawn.raycastOffset + Vector3.Scale(Random.insideUnitSphere, Vector3.Scale(bounds, spawn.boundsMultiplier));
		Vector3 checkDir = spawn.raycastDirection;

		for (int t = 0; t < 10; t++)
		{
			Debug.DrawRay(checkPos, checkDir, Color.red, 50f);
			RaycastHit hit;
			if (Physics.Raycast(checkPos, checkDir, out hit, 500f))
			{
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("World"))
				{
					SpawnResource(spawn.spawnPrefab, hit.point, Quaternion.LookRotation(hit.normal));

					return;
				}
			}
		}

		Debug.Log("Nothing spawned.");
	}

	void SpawnResource (GameObject resource, Vector3 position, Quaternion rotation)
    {
		GameObject obj = Instantiate(resource, position, rotation);
		HiveMind.Instance.AddResource(obj.GetComponent<ResourceHandler>());
		obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
	}
}

[System.Serializable]
public class ResourceSpawn
{
    public GameObject spawnPrefab;
    public Vector3 raycastOffset; //raycast's offset from spawnTarget
    public Vector3 raycastDirection;
	public Vector3 boundsMultiplier;
    public int initialSpawnAmountMin;
    public int initialSpawnAmountMax;
    public int spawnFrequency;
	public float nextSpawnTime;

	public void SetNextSpawnTime ()
    {
		nextSpawnTime = Time.time + spawnFrequency;
    }
}
