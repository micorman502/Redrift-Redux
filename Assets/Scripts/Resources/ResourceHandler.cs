using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ResourceHandler : MonoBehaviour, IItemSaveable, IResource {
	[Header("Save Info")]
	[SerializeField] bool dontSave;
	[SerializeField] string saveID;
	[SerializeField] bool dontRegisterToHivemind;

	[Header("Resource Info")]
	[SerializeField] Resource resource;
	[SerializeField] int health;

	[Header("Visual")]
	[SerializeField] GameObject gatherFX;
	[SerializeField] GameObject deathObject;
	[SerializeField] GameObject optionalVisual;

	bool loaded;

	void Awake ()
	{
		PreLoadInitialise();
	}

	void Start ()
    {
		if (loaded)
			return;

		PostLoadInitialise();
		loaded = true;
	}

	protected virtual void PreLoadInitialise ()
    {
		if (!dontRegisterToHivemind)
		{
			HiveMind.Instance.AddResource(this);
		}
		if (health == 0)
		{
			health = resource.maxGathers;
		}
	}

	protected virtual void PostLoadInitialise ()
    {

    }

	protected virtual WorldItem[] Gather (int gatherMult)
	{
		List<WorldItem> returnedItems = new List<WorldItem>();
		int i = 0;
		foreach (ItemInfo item in resource.resourceItems)
		{
			if (Random.Range(0f, 1f) <= resource.chances[i])
			{
				returnedItems.Add(new WorldItem(item, gatherMult));
			}
			i++;
		}

		health -= 1;
		if (health <= 0 && !resource.infiniteGathers)
		{
			DestroyResource();
		} else
        {
			GatherFX();
        }

		return returnedItems.ToArray();
	}

	public virtual void DestroyResource ()
	{
		if (!dontRegisterToHivemind)
		{
			HiveMind.Instance.RemoveResource(this);
		}

		DestroyFX();

		Destroy(gameObject);
	}

	protected virtual void GatherFX ()
	{
		if (optionalVisual)
		{
			optionalVisual.transform.DOShakeRotation(0.5f, 5f);
		}
		if (gatherFX)
		{
			Destroy(Instantiate(gatherFX, transform.position, transform.rotation), 7f);
		}
	}

	protected virtual void DestroyFX ()
	{
		if (deathObject)
		{
			Destroy(Instantiate(deathObject, transform.position, transform.rotation), 7f);
		}
		if (optionalVisual)
		{
			optionalVisual.transform.DOKill();
		}
	}

	public void SetHealth (int _health)
    {
		health = _health;
    } 

	public Resource GetResource ()
    {
		return resource;
    }

	public int GetHealth ()
    {
		return health;
    }

	public WorldItem[] HandGather()
	{
		return Gather(1);
	}

	public WorldItem[] ToolGather(ToolInfo tool)
	{
		return Gather(tool.gatherAmountMult);
	}

	public virtual void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, dontSave ? -1 : ObjectDatabase.GetIntegerID(saveID));

		newData.num = health;

		data = newData;
		objData = newObjData;
		_dontSave = dontSave;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		Load(data, objData);
		PostLoadInitialise();
	}

	protected virtual void Load (ItemSaveData data, ObjectSaveData objData)
    {
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		health = data.num;

		loaded = true;
	}
}
