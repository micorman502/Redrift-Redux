using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour, IItemSaveable, IHotText, IResource {
	[SerializeField] bool dontSave;
	[SerializeField] string saveID;
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

	public void HideHotText ()
    {
		HotTextManager.Instance.RemoveHotText("resource");
	}

	public void ShowHotText ()
    {
		HotTextManager.Instance.ReplaceHotText(new HotTextInfo(" to gather <" + resource.resourceName + ">", KeyCode.Mouse0, 6, "resource"));
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
			{ // This resource is a tree
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
