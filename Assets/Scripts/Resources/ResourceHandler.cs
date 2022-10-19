using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour, IItemSaveable, IResource {
	[SerializeField] bool dontSave;
	[SerializeField] string saveID;
	[SerializeField] bool dontRegisterToHivemind;
	[SerializeField] GameObject deathObject;
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
			if (resource.resourceName == "Tree")
			{ // This resource is a tree
				GetComponent<TreeResource>().DropFruits();
			}
			DestroyResource();
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
			if (resource.resourceName == "Tree")
			{ // This resource is a tree //dumbest comment in existence
				GetComponent<TreeResource>().DropFruits();
			}
			DestroyResource();
		}

		return returnedItems.ToArray();
	}

	public virtual void DestroyResource ()
	{
		if (!dontRegisterToHivemind)
		{
			HiveMind.Instance.RemoveResource(this);
		}
		if (deathObject)
		{
			Destroy(Instantiate(deathObject, transform.position, transform.rotation), 10);
		}
		Destroy(gameObject);
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, dontSave ? -1 : ObjectDatabase.Instance.GetIntID(saveID));

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
