using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMiner : MonoBehaviour, IItemSaveable, IItemInteractable {

	[SerializeField] string saveID;

	NavMeshAgent agent;

	HiveMind hive;

	public GameObject toolHolder;

	[HideInInspector] public ResourceHandler target;

	float interactRange = 2.5f;

	Animator animator;

	bool moving = false;
	bool gathering = false;

	public List<WorldItem> items = new List<WorldItem>();

	float gatherTime = 0f;

	public ToolInfo currentToolItem = null;
	GameObject currentToolObj = null;

	PlayerController player;

	void Start() {
		hive = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<HiveMind>();
		player = FindObjectOfType<PlayerController>();
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		GetComponent<AudioSource>().outputAudioMixerGroup = FindObjectOfType<SettingsManager>().audioMixer.FindMatchingGroups("Master")[0];
	}
	
	void FixedUpdate() {
		if(agent.isOnNavMesh) {
			if(currentToolItem) {
				if(target) {
					if(!moving) {
						moving = true;
						animator.SetBool("Moving", moving);
					}
					if(Vector3.Distance(transform.position, target.transform.position) <= interactRange) {
						if(!gathering) {
							gathering = true;
							animator.SetBool("Gathering", gathering);
						}
						if(moving) {
							moving = false;
							animator.SetBool("Moving", moving);
						}
						gatherTime += Time.deltaTime * currentToolItem.gatherSpeedMult;
						if(gatherTime >= target.resource.gatherTime) {
							WorldItem[] gatheredItems = target.ToolGather(currentToolItem);
							foreach (WorldItem gathered in gatheredItems)
							{
								AddItem(gathered.item, gathered.amount);
							}
							gatherTime = 0f;
						}
					}
				} else {
					if(gathering) {
						gathering = false;
						gatherTime = 0f;
						animator.SetBool("Gathering", gathering);
					}

					ResourceHandler closestHandler = null;
					float closestDistance = Mathf.Infinity;
					foreach(ResourceHandler resourceHandler in hive.worldResources) {
						if(resourceHandler) {
							float dist = Vector3.Distance(transform.position, resourceHandler.transform.position);
							if(dist < closestDistance) {
								closestDistance = dist;
								closestHandler = resourceHandler;
							}
						}
					}

					if(closestHandler != null) {
						target = closestHandler;
						if(agent.isStopped) {
							agent.isStopped = false;
						}
						//target = hive.worldResources[Random.Range(0, hive.worldResources.Count)];
						agent.SetDestination(target.transform.position);
						if(Vector3.Distance(agent.destination, target.transform.position) > interactRange) {
							target = null; 
						}
					} else {
						if(!agent.isStopped) {
							agent.isStopped = true;
							moving = false;
							animator.SetBool("Moving", moving);
						}
					}
				}
			}
		} else {
			//Debug.Log("Not on NavMesh");
		}
	}

	public void Interact (WorldItem item)
    {
		SetTool(item.item);
    }

	public void SetTool(ItemInfo item) {
		PlayerInventory inventory = PlayerController.currentPlayer.gameObject.GetComponent<PlayerInventory>();

		if (!item)
			return;
		if (!(item is ToolInfo))
			return;
		inventory.inventory.RemoveItem(new WorldItem(item, 1));

		if (currentToolItem)
		{
			inventory.inventory.AddItem(new WorldItem(currentToolItem, 1));
		}

		if (currentToolObj) {
			Destroy(currentToolObj);
		}

		currentToolItem = item as ToolInfo;
		GameObject obj = Instantiate(item.droppedPrefab, toolHolder.transform);

		Rigidbody objRB = obj.GetComponent<Rigidbody>();
		if(objRB) {
			objRB.isKinematic = true;
		}

		ArtificialInertia inertia = obj.GetComponent<ArtificialInertia>();
		if (inertia)
        {
			inertia.root = transform;
        }
		obj.tag = "Untagged";
		foreach(Transform trans in obj.transform) {
			trans.tag = "Untagged";
		}

		currentToolObj = obj;
	}

	public ItemInfo GatherTool() {
		ItemInfo returnItem = currentToolItem;
		currentToolItem = null;
		Destroy(currentToolObj);
		return returnItem;
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

		if (currentToolItem)
		{
			newData.itemID = currentToolItem.id;
		}
		else
		{
			newData.itemID = -1;
		}

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
		if (data.itemID != -1)
		{
			SetTool(ItemDatabase.Instance.GetItem(data.itemID));
		}
	}
}
