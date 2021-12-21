using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour, IItemSaveable, INoticeText, IResource {
	[SerializeField] bool dontSave;
	[SerializeField] int saveID;
	[SerializeField] bool dontRegisterToHivemind;
	public Resource resource;

	public int health;

	void Awake() {
		if (!dontRegisterToHivemind)
		{
			HiveMind.Instance.AddResource(this);
		}
	}

	void Start ()
    {
		if (health == 0)
		{
			health = resource.maxGathers;
		}
	}

	public string GetNoticeText ()
    {
		return "Hold [LMB] to gather [" + resource.resourceName + "]";
    }

	public void SetNoticeText (string dummy)
    {

    }

	public Resource GetResource ()
    {
		return resource;
    }

	public WorldItem[] HandGather()
	{
		List<WorldItem> returnedItems = new List<WorldItem>();
		int i = 0;
		foreach (ItemInfo item in resource.resourceItems)
		{
			if (Random.Range(0f, 1f) <= resource.chances[i])
			{
				returnedItems.Add(new WorldItem(item, 1));
			}
			i++;
		}

		health -= 1;
		if (health <= 0 && !resource.infiniteGathers)
		{
			if (!dontRegisterToHivemind)
			{
				HiveMind.Instance.RemoveResource(this);
			}
			if (resource.id == 5)
			{ // This resource is a tree
				GetComponent<TreeResource>().DropFruits();
			}
			Destroy(gameObject);
		}

		return returnedItems.ToArray();
	}

	public WorldItem[] ToolGather(ToolInfo tool)
	{
		List<WorldItem> returnedItems = new List<WorldItem>();
		int i = 0;
		foreach (ItemInfo item in resource.resourceItems)
		{
			if (Random.Range(0f, 1f) <= resource.chances[i])
			{
				returnedItems.Add(new WorldItem(item, tool.gatherAmountMult));
			}
			i++;
		}

		health -= 1;
		if (health <= 0 && !resource.infiniteGathers)
		{
			if (!dontRegisterToHivemind)
			{
				HiveMind.Instance.RemoveResource(this);
			}
			if (resource.id == 5)
			{ // This resource is a tree
				GetComponent<TreeResource>().DropFruits();
			}
			Destroy(gameObject);
		}

		return returnedItems.ToArray();
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		newData.num = health;

		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		health = data.num;
	}
}
