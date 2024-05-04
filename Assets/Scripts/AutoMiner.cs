using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMiner : MonoBehaviour, IItemSaveable, IInteractable, IItemPickup, IHotText {

	[SerializeField] string saveID;
	[SerializeField] ParticleSystem exhaustParticles;

	[SerializeField] float gatherRange = 2.5f;
	[SerializeField] ToolInfo tool;

	NavMeshAgent agent;

	[HideInInspector] public ResourceHandler target;

	Animator animator;

	bool moving = false;
	bool gathering = false;

	List<WorldItem> items = new List<WorldItem>();

	float gatherTime = 0f;

	int ticksPerCheck = 25;
	int curTick = 0;

	void Start() {
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}

	public WorldItem[] GetItems ()
    {
		List<WorldItem> pickupItems = items;

		return pickupItems.ToArray();
    }

	public void Pickup ()
    {
		items.Clear(); //kinda useless but here, idk... just in case?
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
				if (gatherTime >= target.GetResource().gatherTime)
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
		foreach (ResourceHandler resourceHandler in HiveMind.Instance.worldResources)
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
			int amt = Mathf.Clamp(inv.SpaceLeftForItem(items[i]), 1, items[i].amount);
			if (amt == items[i].amount)
            {
				inv.AddItem(items[i]);
				items.RemoveAt(i);
				continue;
            } 
			
			if (amt > 0)
            {
				inv.AddItem(new WorldItem(items[i].item, amt));
				items[i].amount -= amt;
				continue;
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
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

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
		List<ItemInfo> newItems = SaveManager.Instance.IDsToItems(data.itemIDs);
		for (int i = 0; i < newItems.Count; i++)
        {
			items.Add(new WorldItem(newItems[i], data.itemAmounts[i]));
        }
	}

	public void HideHotText ()
	{
		HotTextManager.Instance.RemoveHotText(new HotTextInfo("", KeyCode.F, HotTextInfo.Priority.Interact, "autominerInteract"));
	}

	public void ShowHotText ()
	{
		//string infoText = (sortingItem ? sortingItem.name : "") + (blackListEnabled ? " (Blacklist)" : " (Whitelist)");
		HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Collect", KeyCode.F, HotTextInfo.Priority.Interact, "autominerInteract"));
	}

	public void UpdateHotText ()
	{
		//string infoText = (sortingItem ? sortingItem.name : "") + (blackListEnabled ? " (Blacklist)" : " (Whitelist)");
		HotTextManager.Instance.UpdateHotText(new HotTextInfo("Collect", KeyCode.F, HotTextInfo.Priority.Interact, "autominerInteract"));
	}
}
