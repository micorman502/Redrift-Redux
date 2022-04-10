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

	public float smallIslandSpawnTime = 60f;
	[SerializeField] float smallIslandSpawnVariationAmt;
	float nextSmallIslandSpawnTime;

	int difficulty;

	bool gameStarted;

	PersistentData persistentData;

	void Start() {
		persistentData = FindObjectOfType<PersistentData>();
		if(persistentData) {
			difficulty = persistentData.difficulty;
			if(!persistentData.loadingFromSave) {
				GenerateWorld();
			}
		} else {
			GenerateWorld();
		}
		SetUpWorld();
	}

	void SetUpWorld() {

		if(spawnSmallIslands) {
			nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			SpawnSmallIsland();
		}
	}

	void GenerateWorld() {

		if (!starterCratePrefab)
			return;
		if(difficulty == 0 && worldType == WorldType.Light) {
			for(int d = 0; d < 2; d++) {
				Instantiate(starterCratePrefab, Vector3.up * 4f + Vector3.up * d, starterCratePrefab.transform.rotation);
			}
		}
	}

	void Update() {

		if(spawnSmallIslands) {
			if(Time.time >= nextSmallIslandSpawnTime) {
				SpawnSmallIsland();
				nextSmallIslandSpawnTime = Time.time + smallIslandSpawnTime;
			}
		}
	}

	void SpawnSmallIsland() {
		GameObject islandObj = Instantiate(smallIslandPrefab, transform.TransformPoint(smallIslandSpawnLocations[Random.Range(0, smallIslandSpawnLocations.Length)]) + Random.insideUnitSphere * smallIslandSpawnVariationAmt, smallIslandPrefab.transform.rotation);
		Vector3 pos = new Vector3(Random.Range(-1f, 1f), 300f, Random.Range(-1f, 1f)) + islandObj.transform.position;

		RaycastHit hit;

		if(Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, -1)) {
			GameObject crateObj = Instantiate(cratePrefab, hit.point + Vector3.up * 0.5f, Quaternion.LookRotation(hit.normal));
			crateObj.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
			crateObj.GetComponent<ArtificialInertia>().SetRootParent(islandObj.transform);
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
