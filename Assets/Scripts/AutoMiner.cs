using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMiner : MonoBehaviour, IItemSaveable, IInteractable, IItemPickup {

	[SerializeField] string saveID;
	[SerializeField] ParticleSystem exhaustParticles;

	[SerializeField] float gatherRange = 2.5f;
	[SerializeField] ToolInfo tool;

	NavMeshAgent agent;

	HiveMind hive;

	[HideInInspector] public ResourceHandler target;

	Animator animator;

	bool moving = false;
	bool gathering = false;

	List<WorldItem> items = new List<WorldItem>();

	float gatherTime = 0f;

	int ticksPerCheck = 25;
	int curTick = 0;

	void Start() {
		hive = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<HiveMind>();
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		GetComponent<AudioSource>().outputAudioMixerGroup = FindObjectOfType<SettingsManager>().audioMixer.FindMatchingGroups("Master")[0];
	}

	public WorldItem[] Pickup ()
    {
		List<WorldItem> pickupItems = items;

		return pickupItems.ToArray();
    }

	void FixedUpdate ()
	{
		if (!agent.isOnNavMesh)
			return;
		curTick++;
		if (curTick > ticksPerCheck)
		{
			curTick = 0;
			ResourceCheck();
		}

		if (moving || gathering)
        {
			ParticleSystem.EmissionModule mod = exhaustParticles.emission;
			mod.rateOverTimeMultiplier = 1;
        } else
        {
			ParticleSystem.EmissionModule mod = exhaustParticles.emission;
			mod.rateOverTimeMultiplier = 0;
		}

		if (target)
		{
			if (!moving)
			{
				moving = true;
				animator.SetBool("Moving", moving);
			}
			if (Vector3.Distance(transform.position, target.transform.position) <= gatherRange)
			{
				if (!gathering)
				{
					gathering = true;
					animator.SetBool("Gathering", gathering);
				}
				if (moving)
				{
					moving = false;
					animator.SetBool("Moving", moving);
				}
				gatherTime += Time.deltaTime * tool.gatherSpeedMult;
				if (gatherTime >= target.resource.gatherTime)
				{
					WorldItem[] gatheredItems = target.ToolGather(tool);
					foreach (WorldItem gathered in gatheredItems)
					{
						AddItem(gathered.item, gathered.amount);
					}
					gatherTime = 0f;
					exhaustParticles.Emit(6);
				}
			}
		} else
        {
			if (gathering)
			{
				gathering = false;
				gatherTime = 0f;
				animator.SetBool("Gathering", gathering);
			}
		}
	}

	void ResourceCheck ()
    {
		ResourceHandler closestHandler = null;
		float closestDistance = Mathf.Infinity;
		foreach (ResourceHandler resourceHandler in hive.worldResources)
		{
			if (resourceHandler)
			{
				float dist = Vector3.Distance(transform.position, resourceHandler.transform.position);
				if (dist < closestDistance)
				{
					closestDistance = dist;
					closestHandler = resourceHandler;
				}
			}
		}

		if (closestHandler != null)
		{
			target = closestHandler;
			if (agent.isStopped)
			{
				agent.isStopped = false;
			}
			agent.SetDestination(target.transform.position);
			if (Vector3.Distance(agent.destination, target.transform.position) > gatherRange)
			{
				target = null;
			}
		}
		else
		{
			if (!agent.isStopped)
			{
				agent.isStopped = true;
				moving = false;
				animator.SetBool("Moving", moving);
			}
		}
	}

	public void Interact ()
    {
		Inventory inv = PlayerController.currentPlayer.gameObject.GetComponent<PlayerInventory>().inventory;

		for (int i = items.Count - 1; i >= 0; i--)
        {
			int amt = inv.SpaceLeftForItem(items[i]);
			if (amt == items[i].amount)
            {
				inv.AddItem(items[i]);
				items.RemoveAt(i);
            } else if (amt > 0)
            {
				inv.AddItem(new WorldItem(items[i].item, amt));
				items[i].amount -= amt;
			}
        }
    }

	public void ClearItems() {
		items.Clear();
	}

	void AddItem(ItemInfo _item, int amount) {

		bool hasItem = false;

		for (int i = 0; i < items.Count; i++)
        {
			if (items[i].item.id == _item.id)
            {
				hasItem = true;
                items[i].amount += amount;
            }
        }

		if(!hasItem) {
			items.Add(new WorldItem(_item, amount));
		}
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.Instance.GetIntID(saveID));

		newData.itemIDs = SaveManager.Instance.ItemsToIDs(items);
		List<int> newAmts = new List<int>();
		for (int i = 0; i < items.Count; i++)
        {
			newAmts.Add(items[i].amount);
        }
		newData.itemAmounts = newAmts;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		List<ItemInfo> newItems = SaveManager.Instance.IDsToItems(data.itemIDs);
		for (int i = 0; i < newItems.Count; i++)
        {
			items[i].item = newItems[i];
			items[i].amount = data.itemAmounts[i];
        }
	}
}
