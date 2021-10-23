using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Base", order = 0)]
public class ItemInfo : ScriptableObject
{
	public string itemName;
	public string itemDescription;
	public int stackSize = 100;
	public int id;
	public int achievementId = -1;
	public Sprite icon;
	public GameObject droppedPrefab;
}
