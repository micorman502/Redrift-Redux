using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoMiner : MonoBehaviour, IItemSaveable {

	[SerializeField] int saveID;

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
	
	void Update() {
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
							int i = 0;
							foreach(ItemInfo item in target.resource.resourceItems) {
								if(Random.Range(0f, 1f) <= target.resource.chances[i]) {
									AddItem(item, currentToolItem.gatherAmountMult);
								}
								i++;
							}
							target.Gather(currentToolItem.gatherAmountMult);
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

	public void SetTool(ItemInfo item) {

		if(currentToolObj) {
			Destroy(currentToolObj);
		}

		if(currentToolItem) {
			player.inventory.AddItem(currentToolItem, 1);
		}

		currentToolItem = item as ToolInfo;
		GameObject obj = Instantiate(item.droppedPrefab, toolHolder.transform);

		Rigidbody objRB = obj.GetComponent<Rigidbody>();
		if(objRB) {
			objRB.isKinematic = true;
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
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

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
			SetTool(SaveManager.Instance.FindItem(data.itemID));
		}
	}
}
