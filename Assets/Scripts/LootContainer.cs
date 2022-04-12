using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainer : MonoBehaviour, IInteractable {

	public LootItem[] loot;

	public GameObject lootParticles;
	
	public void Interact ()
    {
		Open();
    }

	public void Open() {
		int i = 0;
		foreach(LootItem lootItem in loot) {
			int amount = Random.Range(lootItem.minAmount, lootItem.maxAmount + 1);
			for(int a = 0; a < amount; a++) {
				if(Random.Range(0f, 1f) <= lootItem.chance) {
					GameObject itemObj = Instantiate(lootItem.item.droppedPrefab, transform.position + Vector3.up * 0.3f + Random.onUnitSphere * 0.2f, lootItem.item.droppedPrefab.transform.rotation);
					Rigidbody itemRB = itemObj.GetComponent<Rigidbody>();
					if(itemRB) {
						itemRB.AddExplosionForce(3f, transform.position, 2f);
					}
				}
			}
			i++;
		}
		GameObject obj = Instantiate(lootParticles, transform.position, Quaternion.identity);
		Destroy(obj, 10f);
		Destroy(gameObject);
	}
}

[System.Serializable]
public struct LootItem {
	public ItemInfo item;
	public int minAmount;
	public int maxAmount;
	public float chance;
}