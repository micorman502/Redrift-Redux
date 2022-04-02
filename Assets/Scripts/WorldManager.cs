using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

	public enum WorldType {Light, Dark}

	public WorldType worldType;

	public bool spawnSmallIslands = true;
	public GameObject smallIslandPrefab;

	public GameObject world;
	public GameObject starterCratePrefab;
	public GameObject cratePrefab;

	public float bounds;

	public Vector3[] smallIslandSpawnLocations;
	//public GameObject copperOreResourcePrefab;
	[SerializeField] Spawn[] resourceSpawns;

	float[] nextSpawnTime;

	public float smallIslandSpawnTime = 60f;
	float nextSmallIslandSpawnTime;

	int difficulty;

	bool gameStarted;

	PersistentData persistentData;

	void Start() {
		persistentData = FindObjectOfType<PersistentData>();
		if(persistentData) {
			difficulty = persistentData.difficulty;
			if(!persistentData.loadSave) {
				GenerateWorld();
			}
		} else {
			GenerateWorld();
		}
		SetUpWorld();
	}

	void SetUpWorld() {
		nextSpawnTime = new float[resourceSpawns.Length];

		for (int i = 0; i < resourceSpawns.Length; i++)
        {
			nextSpawnTime[i] = Time.time + resourceSpawns[i].spawnFrequency;
        }

		if(spawnSmallIslands) {
			nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			SpawnSmallIsland();
		}
	}

	void GenerateWorld() {

		for (int i = 0; i < resourceSpawns.Length; i++)
        {
			int initialAmount = Random.Range(resourceSpawns[i].initialSpawnAmountMin, resourceSpawns[i].initialSpawnAmountMax + 1);

			for (int r = 0; r < initialAmount; r++)
            {
				Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
				RaycastHit hit;
				if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
				{
					if (hit.collider.gameObject == world)
					{
						GameObject obj = Instantiate(resourceSpawns[i].spawnPrefab, hit.point, Quaternion.LookRotation(hit.normal));
						ResourceHandler handler = obj.GetComponent<ResourceHandler>();
						if (handler)
						{
							HiveMind.Instance.AddResource(handler);

						}
						obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
					}
				}
			}
		}

		if (!starterCratePrefab)
			return;
		if(difficulty == 0 && worldType == WorldType.Light) {
			for(int d = 0; d < 2; d++) {
				Instantiate(starterCratePrefab, Vector3.up * 4f + Vector3.up * d, starterCratePrefab.transform.rotation);
			}
		}
	}

	void Update() {

		for (int i = 0; i < nextSpawnTime.Length; i++)
        {
			if (Time.time >= nextSpawnTime[i])
			{
				Vector3 pos = transform.TransformPoint(new Vector3(Random.insideUnitCircle.x * bounds, 300f, Random.insideUnitCircle.y * bounds));
				RaycastHit hit;
				if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1))
				{
					if (hit.collider.gameObject == world)
					{
						GameObject obj = Instantiate(resourceSpawns[i].spawnPrefab, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
						HiveMind.Instance.AddResource(obj.GetComponent<ResourceHandler>());
						obj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
					}
					else
					{
						continue;
					}
				}

				nextSpawnTime[i] = Time.time + resourceSpawns[i].spawnFrequency;
			}
		}

		if(spawnSmallIslands) {
			if(Time.time >= nextSmallIslandSpawnTime) {
				SpawnSmallIsland();
				nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			}
		}
	}

	void SpawnSmallIsland() {
		GameObject islandObj = Instantiate(smallIslandPrefab, transform.TransformPoint(smallIslandSpawnLocations[Random.Range(0, smallIslandSpawnLocations.Length)]), smallIslandPrefab.transform.rotation) as GameObject;
		Vector3 pos = new Vector3(Random.Range(-1f, 1f), 300f, Random.Range(-1f, 1f)) + islandObj.transform.position;
		RaycastHit hit;
		if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
			GameObject crateObj = Instantiate(cratePrefab, hit.point + Vector3.up * 0.5f, Quaternion.LookRotation(hit.normal)) as GameObject;
			crateObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		}
		//if(Random.Range(0, 2) == 0) {
		//	if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
		//		GameObject oreObj = Instantiate(copperOreResourcePrefab, hit.point, Quaternion.LookRotation(hit.normal), islandObj.transform) as GameObject;
		//		oreObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
		//	}
		//} else {
			
		//}
	}
}

[System.Serializable]
public struct Spawn
{
	public GameObject spawnPrefab;
	public int initialSpawnAmountMin;
	public int initialSpawnAmountMax;
	public int spawnFrequency;
}
