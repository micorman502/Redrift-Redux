using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour {
	public static SaveManager Instance { get; private set; }

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

	bool disableSaving;

	void Awake() {
		if (!GameManager.gameInitialised)
			return;

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
				Debug.Log("Loading save #" + PersistentData.Instance.saveToLoad);
				LoadGame(PersistentData.Instance.saveToLoad);
			}
			else
			{
				difficulty = PersistentData.Instance.difficulty;
				mode = PersistentData.Instance.mode;
				SaveGame();
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

			try
			{
				Save save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(path));

				try
				{
					PersistentData.Instance.difficulty = save.difficulty;
					PersistentData.Instance.mode = save.mode;
					PersistentData.Instance.loadingFromSave = true;
				}
				catch (Exception e)
				{
					Debug.LogWarning("Error while initialising PersistentData in SaveManager.LoadGame, Caught exception " + e.Message);
				}

				try
				{
					if (save.mode == 0)
					{
						inventory.DefaultSetup();
					}
					else
					{
						inventory.LoadCreativeMode();
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning("Error while initialising inventory in SaveManager.LoadGame, Caught exception " + e.Message);
				}

				for (int i = 0; i < save.inventoryItems.Count; i++)
				{
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
				player.SetVitals(save.playerHealth, save.playerHunger);

				if (save.playerDead)
				{
					player.Die();
				}
				else
				{
					RealmTeleportManager.Instance.LoadFromSave(save.realmIndex);
				}

				player.rb.velocity = save.playerVelocity;

				difficulty = save.difficulty;
				mode = save.mode;

				AchievementManager.Instance.SetAchievements(save.achievementIDs);

				inventory.InventoryUpdate();

				for (int i = 0; i < save.savedObjects.Count; i++) //load up items and such last, to prevent a single error from breaking the loading loop 
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

				saveText.text = "Game loaded from " + save.saveTime.ToString("HH:mm MMMM dd, yyyy");
			} catch (Exception e)
            {
				saveText.text = "Error while loading. Saving disabled.";
				Debug.LogWarning("Error while loading. Saving disabled. Caught exception: " + e.Message + ", with stack trace: " + e.StackTrace);
				disableSaving = true;
            }
		} else {
			saveText.text = "No game save found";
		}
	}

	public void SaveGame() {
		if (disableSaving)
			return;

		canvasAnim.SetTrigger("Save");
		CheckSaveDirectory();
		Save save = CreateSave();

		string path;
		if(PersistentData.Instance.loadingFromSave) {
			path = GetSavePath(info[PersistentData.Instance.saveToLoad].Name, true);
		} else {
			path = GetSavePath(PersistentData.Instance.newSaveName, false);
		}
		File.WriteAllText(path, JsonConvert.SerializeObject(save));

		saveText.text = "Game saved at " + DateTime.Now.ToString("HH:mm MMMM dd, yyyy");
	}

	private Save CreateSave() {
		Save save = new Save();

		save.playerTransform = new ObjectSaveData(player.transform.position, player.transform.rotation, 0);
		player.GetVitals(out float maxHealth, out float health);
		save.playerHealth = health;

		save.saveTime = DateTime.Now;

		save.realmIndex = RealmTeleportManager.Instance.GetCurrentRealmIndex();

		//inventory.DefaultSetup();

		foreach(InventorySlot slot in inventory.inventory.Slots) {
			if(slot.Item) {
				save.inventoryItems.Add(new WorldItem(slot.Item, slot.Count));
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

	public static string GetSavePath (string saveName, bool nameIncludesFileExtension)
    {
		if (nameIncludesFileExtension)
		{
			return Application.persistentDataPath + "/saves/" + saveName;
		}
		else
		{
			return Application.persistentDataPath + "/saves/" + saveName + ".save";
		}
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