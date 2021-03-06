using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour {
	public static SaveManager Instance;

	[SerializeField] Animator canvasAnim;

	public Text saveText;

	PlayerInventory inventory;
	PlayerController player;

	WorldManager worldManager;

	public int difficulty;
	public int mode;

	FileInfo[] info;

	bool autoSave = true;
	float autoSaveInterval = 120f;
	float autoSaveTimer = 0f;

	void Awake() {
		if (Instance)
        {
			Destroy(this);
			return;
        }
		Instance = this;
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		worldManager = FindObjectOfType<WorldManager>();

		inventory = playerObj.GetComponent<PlayerInventory>();
		player = playerObj.GetComponent<PlayerController>();

		CheckSaveDirectory();
		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
		info = dir.GetFiles("*.*");

		if (autoSave)
		{
			autoSaveTimer = autoSaveInterval;
		}

		if (PersistentData.Instance)
		{

			if (PersistentData.Instance.loadingFromSave)
			{
				Debug.Log("loading");
				LoadGame(PersistentData.Instance.saveToLoad);
			}
			else
			{
				difficulty = PersistentData.Instance.difficulty;
				mode = PersistentData.Instance.mode;
				SaveGame();
			}

			if (mode == 1)
			{ // Creative mode
				if (!PersistentData.Instance.loadingFromSave)
				{
					inventory.LoadCreativeMode();
					player.LoadCreativeMode();
				}
			}
		}
	}

	void Update ()
	{
		if (!autoSave)
			return;

		if (Time.time > autoSaveTimer)
		{
			SaveGame();
			autoSaveTimer = Time.time + autoSaveInterval;
		}
	}

	public List<ItemInfo> IDsToItems(List<int> IDs) {
		List<ItemInfo> items = new List<ItemInfo>();
		foreach(int itemID in IDs) {
			items.Add(ItemDatabase.Instance.GetItem(itemID));
		}
		return items;
	}

	public List<int> ItemsToIDs(List<ItemInfo> items) {
		List<int> IDs = new List<int>();
		foreach(ItemInfo item in items) {
			IDs.Add(item.id);
		}
		return IDs;
	}

	public List<int> ItemsToIDs(List<WorldItem> items)
	{
		List<int> IDs = new List<int>();
		foreach (WorldItem item in items)
		{
			IDs.Add(item.item.id);
		}
		return IDs;
	}

	void CheckSaveDirectory() {
		if(!Directory.Exists(Application.persistentDataPath + "/saves")) {
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}
	}

	public void LoadGame(int saveNum) {
		CheckSaveDirectory();
		string path = Application.persistentDataPath + "/saves/" + info[saveNum].Name;
		if (File.Exists(path)) {
			ClearWorld();

			Save save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(path));

			for (int i = 0; i < save.savedObjects.Count; i++)
            {
				ObjectSaveData newObjData = save.savedObjects[i];
				ItemSaveData newData = save.savedObjectsInfo[i];
				GameObject newObj = Instantiate(ObjectDatabase.Instance.GetObject(newObjData.objectID), newObjData.position, newObjData.rotation);
				IItemSaveable[] saveables = newObj.GetComponents<IItemSaveable>();
				for (int s = 0; s < saveables.Length; s++)
				{
					saveables[s].SetData(newData, newObjData);
				}
            }

			if (save.mode == 1)
            {
				inventory.ManualSetupInventorySize(ItemDatabase.Instance.GetAllItems().Length);
				player.LoadCreativeMode();
			} else
            {
				inventory.DefaultSetup();
            }

			for (int i = 0; i < save.inventoryItems.Count; i++) {
				int id = save.inventoryItems[i].id;
				ItemInfo item = null;
				if (id >= 0)
				{
					item = ItemDatabase.Instance.GetItem(id);
				}
				inventory.inventory.SetSlot(new WorldItem(item, save.inventoryItems[i].amount), i);
			}


			player.transform.position = save.playerTransform.position;
			player.transform.rotation = save.playerTransform.rotation;
			player.hunger = save.playerHunger;
			player.health = save.playerHealth;
			if (save.playerDead) {
				player.Die();
			} else {
				RealmTeleportManager.Instance.LoadFromSave(save.realmIndex);
			}

			player.rb.velocity = save.playerVelocity;

			difficulty = save.difficulty;
			mode = save.mode;

			AchievementManager.Instance.SetAchievements(save.achievementIDs);

			inventory.InventoryUpdate();

			saveText.text = "Game loaded from " + save.saveTime.ToString("HH:mm MMMM dd, yyyy");
		} else {
			saveText.text = "No game save found";
		}
	}

	public void SaveGame() {
		canvasAnim.SetTrigger("Save");
		CheckSaveDirectory();
		Save save = CreateSave();

		string path;
		if(PersistentData.Instance.loadingFromSave) {
			path = Application.persistentDataPath + "/saves/" + info[PersistentData.Instance.saveToLoad].Name;
		} else {
			path = Application.persistentDataPath + "/saves/" + PersistentData.Instance.newSaveName + ".save";
		}
		File.WriteAllText(path, JsonConvert.SerializeObject(save));

		saveText.text = "Game saved at " + DateTime.Now.ToString("HH:mm MMMM dd, yyyy");
	}

	private Save CreateSave() {
		Save save = new Save();

		save.playerTransform = new ObjectSaveData(player.transform.position, player.transform.rotation, 0);
		save.playerHealth = player.health;
		save.playerHunger = player.hunger;
		save.saveTime = DateTime.Now;

		save.realmIndex = RealmTeleportManager.Instance.GetCurrentRealmIndex();

		foreach(InventorySlot item in inventory.inventory.Slots) {
			if(item.Item) {
				save.inventoryItems.Add(new WorldItem(item.Item, item.Count));
			} else {
				save.inventoryItems.Add(new SerializedWorldItem(-1, -1));
			}
		}

		foreach (IItemSaveable saveable in FindInterfaces.Find<IItemSaveable>())
        {
			saveable.GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave);
			if (dontSave || objData.objectID == -1)
				continue;
			save.savedObjects.Add(objData);
			save.savedObjectsInfo.Add(data);
		}

		save.achievementIDs = AchievementManager.Instance.ObtainedAchievements();

		save.difficulty = difficulty;
		save.mode = mode;

		save.playerDead = player.dead;

		save.playerVelocity = player.rb.velocity;

		return save;
	}

	void ClearWorld() {
		foreach(GameObject itemObj in GameObject.FindGameObjectsWithTag("Item")) {
			Destroy(itemObj);
		}

		foreach(GameObject resourceObj in GameObject.FindGameObjectsWithTag("Resource")) {
			if(resourceObj.GetComponent<ResourceHandler>().resource.prefab) {
				Destroy(resourceObj);
			}
		}
		inventory.DefaultSetup();
		inventory.ClearInventory();
	}
}

[System.Serializable]
public struct SerializableVector3 {

	public float x;
	public float y;
	public float z;

	public SerializableVector3(float rX, float rY, float rZ) {
		x = rX;
		y = rY;
		z = rZ;
	}

	public override string ToString() {
		return String.Format("[{0}, {1}, {2}]", x, y, z);
	}

	public static implicit operator Vector3(SerializableVector3 rValue) {
		return new Vector3(rValue.x, rValue.y, rValue.z);
	}

	public static implicit operator SerializableVector3(Vector3 rValue) {
		return new SerializableVector3(rValue.x, rValue.y, rValue.z);
	}
}

[System.Serializable]
public struct SerializableQuaternion {

	public float x;
	public float y;
	public float z;
	public float w;

	public SerializableQuaternion(float rX, float rY, float rZ, float rW) {
		x = rX;
		y = rY;
		z = rZ;
		w = rW;
	}

	public override string ToString() {
		return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
	}

	public static implicit operator Quaternion(SerializableQuaternion rValue) {
		return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
	}

	public static implicit operator SerializableQuaternion(Quaternion rValue) {
		return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
	}
}

// https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html