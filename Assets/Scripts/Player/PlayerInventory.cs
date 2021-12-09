using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {

	[SerializeField] int hotbarSize;
	[SerializeField] GameObject placementParticleSystem;

	public GameObject inventoryContainer;
	public Transform craftingRecipeContainer;
	[SerializeField] GameObject craftingContainer;

	[SerializeField] HeldItem[] heldItems;

	[SerializeField] WorldItem[] items;

	PlayerController player;

	AudioManager audioManager;

	[HideInInspector] public bool placingStructure;
	GameObject currentPreviewObj;
	BuildingInfo currentPlacingItem;
	int currentPlacingRot;

	public int selectedHotbarSlot = 0;
	public ItemInfo currentSelectedItem;

	InventorySlot firstDragSlot;

	SaveManager saveManager;

	int mode;

	float gridX = 1f;
	float gridY = 0.25f;
	float gridZ = 1f;

	[SerializeField] int heldItemIndex = -1;
	int previousHeldItemIndex = -1;

	void Awake() {
		audioManager = FindObjectOfType<AudioManager>();
		saveManager = FindObjectOfType<SaveManager>();

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new WorldItem();
		}

		InventoryEvents.InitialiseInventoryUI(hotbarSize, items.Length);
	}

    private void OnEnable()
    {
		InventoryEvents.StartDrag += BeginDrag;
		InventoryEvents.EndDrag += EndDrag;
		InventoryEvents.RequestInventorySlot += RetrieveInventorySlotToEvent;
	}

    private void OnDisable()
    {
		InventoryEvents.StartDrag -= BeginDrag;
		InventoryEvents.EndDrag -= EndDrag;
		InventoryEvents.RequestInventorySlot -= RetrieveInventorySlotToEvent;
	}

    public void LoadCreativeMode() {
		mode = 1;


		items = new WorldItem[saveManager.allItems.items.Length + 1];

		for (int i = 0; i < items.Length; i++)
        {
			items[i] = new WorldItem();
        }

		InventoryEvents.InitialiseInventoryUI(hotbarSize, items.Length);
		AddAllItems();

		craftingContainer.SetActive(false);
	}

	public void Pickup(ItemHandler itemHandler) {
		if(mode != 1) {
			AddItem(itemHandler.item, 1); //TODO: Check if inventory is full first!
		}
		Destroy(itemHandler.gameObject);
	}

	public void RetrieveInventorySlotToEvent (int index)
    {
		InventoryEvents.UpdateInventorySlot(items[index], index);
    }

	void Update() {

		for(int i = 0; i < hotbarSize; i++) {
			if(Input.GetKeyDown((i + 1).ToString())) {
				if(selectedHotbarSlot != i) {
					SetHotbarSlot(i);
				}
			}
		}

		if((Input.GetAxisRaw("Mouse ScrollWheel") != 0 || Input.GetButtonDown("CycleRight") || Input.GetButtonDown("CycleLeft")) && !player.InMenu()) {
			if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 || Input.GetButtonDown("CycleLeft")) {
				ScrollHotbar(-1);
			} else {
				ScrollHotbar(1);
			}

			HotbarUpdate();
		}
		CheckItemFunctions();
		if(!player.dead) {
			if(placingStructure && !player.InMenu()) {
				player.ShowTooltipText("[LMB] to place, [RMB] to cancel, [R] to rotate");
				if(!currentPreviewObj.activeSelf) {
					currentPreviewObj.SetActive(true);
				}

				if (currentSelectedItem is BuildingInfo)
				{
					BuildingInfo building = currentSelectedItem as BuildingInfo;
					if (!player.target || player.targetHit.distance > player.interactRange)
					{
						if (building.alignToNormal)
						{
							currentPreviewObj.transform.position = player.playerCamera.transform.position + player.playerCamera.transform.forward * player.interactRange;
						}
						else
						{
							Vector3 targetPos = player.playerCamera.transform.position + player.playerCamera.transform.forward * player.interactRange;
							currentPreviewObj.transform.position = new Vector3(Mathf.Round(targetPos.x / gridX) * gridX,
							Mathf.Round(targetPos.y / gridY) * gridY,
							Mathf.Round(targetPos.z / gridZ) * gridZ);
						}
					}
					else
					{
						if (building.alignToNormal)
						{
							currentPreviewObj.transform.position = player.targetHit.point;
						}
						else
						{
							currentPreviewObj.transform.position = new Vector3(Mathf.Round((player.targetHit.point.x / gridX) + player.targetHit.normal.normalized.x * 0.05f) * gridX,
							Mathf.Round((player.targetHit.point.y + player.targetHit.normal.normalized.y * 0.05f) / gridY) * gridY,
							Mathf.Round((player.targetHit.point.z / gridZ) + player.targetHit.normal.normalized.z * 0.05f) * gridZ);
						}
					}

					if (Input.GetKeyDown(KeyCode.R))
					{ //TODO: Make "Rotate" Button, not key
						currentPlacingRot++;
						if (currentPlacingRot >= building.possibleRotations.Length)
						{
							currentPlacingRot = 0;
						}
						if (!building.alignToNormal)
						{
							currentPreviewObj.transform.rotation = Quaternion.Euler(building.possibleRotations[currentPlacingRot]);
						}
					}

					if (building.alignToNormal)
					{
						if (player.target && player.distanceToTarget <= player.interactRange)
						{ // TODO: WORKING ON CURRENTLY |||___|||---|||___|||===================
							currentPreviewObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, player.targetHit.normal) * Quaternion.Euler(currentPlacingItem.possibleRotations[currentPlacingRot]);
						}
						else
						{
							currentPreviewObj.transform.rotation = Quaternion.Euler(building.possibleRotations[currentPlacingRot]);
						}
					}
				}

				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					PlaceBuilding();
				} else if(Input.GetMouseButtonDown(1) || Input.GetAxisRaw("ControllerTriggers") >= 0.1f) {
					StopBuilding();
				}
			}
			if(currentSelectedItem && !player.InMenu()) {
				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					if(currentSelectedItem is BuildingInfo && !placingStructure) {
						StartBuilding(currentSelectedItem);
					}

					InventoryUpdate();
				} else if(Input.GetButtonDown("Drop")) {
					if(Input.GetButton("Supersize")) {
						if(mode != 1) {
							DropItem(currentSelectedItem, Ping(5, items[selectedHotbarSlot].amount));
						} else {
							DropItem(currentSelectedItem, 5);
						}
					} else {
						DropItem(currentSelectedItem, 1);
					}

					InventoryUpdate();
				}
			}
		}

		if (heldItemIndex != -1)
			heldItems[heldItemIndex].ItemUpdate();
	}

	void CheckItemFunctions ()
    {
		if (heldItemIndex >= heldItems.Length)
			return;
		if (Input.GetMouseButtonDown(0))
        {
			if (heldItemIndex != -1)
				heldItems[heldItemIndex].Use();
        }
		if (Input.GetMouseButton(0))
		{
			if (heldItemIndex != -1)
				heldItems[heldItemIndex].UseRepeating();
		}
		if (Input.GetMouseButtonDown(1))
        {
			if (heldItemIndex != -1)
				heldItems[heldItemIndex].AltUse();
        }
		if (Input.GetMouseButton(1))
		{
			if (heldItemIndex != -1)
				heldItems[heldItemIndex].AltUseRepeating();
		} //the heldItemIndex check is done 4 times in case the heldItemIndex changes in the process of doing, say, the Use() functions.
	}

    void FixedUpdate()
    {
		if (heldItemIndex != -1)
			heldItems[heldItemIndex].ItemFixedUpdate();
	}

    void ScrollHotbar (int scrollAmt)
    {
		if (scrollAmt != 0)
        {
			SetHotbarSlot(selectedHotbarSlot + scrollAmt);
        }
	}

	void SetHotbarSlot (int slot)
    {
		if (selectedHotbarSlot == slot)
			return;
		selectedHotbarSlot = slot;
		if (selectedHotbarSlot < 0)
		{
			selectedHotbarSlot = hotbarSize - 1;
		}
		if (selectedHotbarSlot > hotbarSize - 1)
		{
			selectedHotbarSlot = 0;
		}
		HotbarUpdate();
	}

	void HotbarUpdate() {
		InventoryEvents.SetHotbarIndex(selectedHotbarSlot);
		currentSelectedItem = items[selectedHotbarSlot].item;
		EquipHeldItem(currentSelectedItem);
		if (placingStructure)
		{
			StopBuilding();
		}
	}

	void EquipHeldItem (ItemInfo _item)
    {
		for (int i = 0; i < heldItems.Length; i++)
        {
			if (heldItems[i].item == _item)
            {
				EquipHeldItem(i);
				return;
            }
        }
		EquipHeldItem(-1);
    }

	void EquipHeldItem (int _index)
    {
		if (_index == heldItemIndex)
			return;
		if (heldItems.Length == 0)
			return;
		heldItemIndex = _index;

		if (previousHeldItemIndex != -1)
		{
			heldItems[previousHeldItemIndex].SetChildState(false);
		}
		previousHeldItemIndex = heldItemIndex;

		if (_index != -1)
		{
			heldItems[_index].SetChildState(true);
		}

		/*heldItemIndex = _index;

		if (_index > -1)
		{
			heldItems[heldItemIndex].itemGameObject.SetActive(true);
		}

		if (previousHeldItemIndex != -1)
        {
			heldItems[previousHeldItemIndex].itemGameObject.SetActive(false);
        }

		previousHeldItemIndex = heldItemIndex;*/
    }

	public void LeaveHoveredItem() {
		InventoryEvents.LeaveHoveredItem();
	}

	public void BeginDrag(InventorySlot slot) {
		firstDragSlot = slot;
	}

	public void EndDrag(InventorySlot slot) {
		if (firstDragSlot)
		{
			SwapItems(firstDragSlot, slot);
			InventoryUpdate();
			firstDragSlot = null;
		}
	}

	void SwapItems(InventorySlot slotA, InventorySlot slotB) {
		Debug.Log("swapping");
		int first = slotA.GetSlotID();
		int second = slotB.GetSlotID();
		WorldItem itemA = new WorldItem(items[first].item, items[first].amount);
		WorldItem itemB = new WorldItem(items[second].item, items[second].amount);

		items[first] = itemB;
		items[second] = itemA;
		InventoryEvents.UpdateInventorySlot(items[first], first);
		InventoryEvents.UpdateInventorySlot(items[second], second);
		InventoryUpdate();
		HotbarUpdate();
	}

	public void SetSlot (WorldItem item, int index)
    {
		items[index] = item;
		InventoryEvents.UpdateInventorySlot(item, index);
    }

	public void AddItem (WorldItem item)
    {
		AddItem(item.item, item.amount);
    }

	public void AddItem(ItemInfo item, int amount) {
		if (!item || amount == 0)
			return;
		int amountLeft = amount;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].item == item || items[i].item == null)
			{
				items[i].item = item;
				if (items[i].amount + amountLeft <= items[i].item.stackSize)
				{
					items[i].amount += amountLeft;
					amountLeft = 0;
					InventoryEvents.UpdateInventorySlot(items[i], i);
					break;
				} else
                {
					int removed = items[i].item.stackSize - items[i].amount;
					amountLeft -= removed; 
					items[i].amount = items[i].item.stackSize;
					InventoryEvents.UpdateInventorySlot(items[i], i);
					if (amountLeft == 0)
                    {
						break;
                    }
                }
			} else
            {// if the item is not the thing we are trying to add here
				continue;
            }
		}

		if (amountLeft > 0)
        {
			SimpleDropItem(item, amountLeft);
		}

		if(amountLeft != amount) { //if items were actually taken
			AchievementManager.Instance.GetAchievement(item.achievementId);
		}

		InventoryUpdate();
		HotbarUpdate();
	}

	/// <summary>
	/// Returns how many items have been taken. Return value of 0 means no items were taken.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="amount"></param>
	/// <returns></returns>
	public int RemoveItem(ItemInfo item, int amount)
	{
		if (mode == 1)
			return amount;
		if (!item || amount == 0)
			return 0;
		int amountLeft = 0;
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].item == item)
			{
				if (items[i].amount - amountLeft >= 0)
				{
					items[i].amount -= amountLeft;
					amountLeft = amount;
					if (items[i].amount == 0)
                    {
						items[i].Clear();
                    }
					InventoryEvents.UpdateInventorySlot(items[i], i);
					break;
				}
				else
				{
					int removed = Mathf.Abs(items[i].amount - amountLeft);
					amountLeft += removed;
					items[i].Clear();
					InventoryEvents.UpdateInventorySlot(items[i], i);
					if (amountLeft <= 0)
					{
						break;
					}
				}
			}
			else
			{// if the item is not the thing we are trying to remove here
				continue;
			}
		}

		InventoryUpdate();
		HotbarUpdate();

		return amountLeft;
	}

	public int RemoveItem(int index, int amount)
	{
		ItemInfo item = items[index].item;
		return RemoveItem(item, amount);
	}

	public bool RemoveItem(ItemInfo item)
	{
		if (RemoveItem(item, 1) == 0)
        {
			return false; //did not get removed
        } else
        {
			return true; //did get removed
        }
	}

	public void SetItem(WorldItem item, int index)
    {
		if (index > -1 && index < items.Length)
		{
			items[index] = item;
			InventoryEvents.UpdateInventorySlot(item, index);
		}
	}

	public void SetItem(ItemInfo item, int amount, int index) {
		SetItem(new WorldItem(item, amount), index);
	}


	public void SimpleDropItem(ItemInfo item, int amount) { //drop item without removing anything
		for(int i = 0; i < amount; i++) {
			GameObject itemObj = Instantiate(item.droppedPrefab, player.playerCamera.transform.position + player.playerCamera.transform.forward * 1.25f + Vector3.up * i * (item.droppedPrefab.GetComponentInChildren<Renderer>().bounds.size.y + 0.1f), player.playerCamera.transform.rotation);
			Rigidbody itemRB = itemObj.GetComponent<Rigidbody>();
			if(itemRB) {
				itemRB.velocity = player.rb.velocity;
			}
		}
	}

	public void DropItem(ItemInfo item, int amount)
	{
		amount -= RemoveItem(item, amount);
		for (int i = 0; i < amount; i++)
		{
			GameObject itemObj = Instantiate(item.droppedPrefab, player.playerCamera.transform.position + player.playerCamera.transform.forward * 1.25f + Vector3.up * i * (item.droppedPrefab.GetComponentInChildren<Renderer>().bounds.size.y + 0.1f), player.playerCamera.transform.rotation);
			Rigidbody itemRB = itemObj.GetComponent<Rigidbody>();
			if (itemRB)
			{
				itemRB.velocity = player.rb.velocity;
			}
		}
	}

	int Ping(int num, int max) {
		if(num > max) {
			return max;
		} else {
			return num;
		}
	}

	void PlaceBuilding ()
    {
		RemoveItem(currentPlacingItem);
		BuildingInfo building = currentPlacingItem;
		GameObject go = Instantiate(building.placedObject, currentPreviewObj.transform.position, currentPreviewObj.transform.rotation);
		GameObject psgo = Instantiate(placementParticleSystem, go.transform);
		MeshRenderer mr = go.GetComponent<MeshRenderer>();
		if (!mr)
		{
			mr = go.GetComponentInChildren<MeshRenderer>();
		}
		if (mr)
		{
			ParticleSystem ps = psgo.GetComponent<ParticleSystem>();
			ParticleSystem.ShapeModule shape = ps.shape;
			shape.meshRenderer = mr;
			ps.Play();
		}
		audioManager.Play("Build");
		StopBuilding();
		InventoryUpdate();
	}

	public void StopBuilding()
	{
		Destroy(currentPreviewObj);
		currentPlacingRot = 0;
		placingStructure = false;
		currentPlacingItem = null;
		currentPreviewObj = null;
		player.HideTooltipText();
	}

	public void StartBuilding(ItemInfo item) {
		if(placingStructure) {
			StopBuilding();
			player.HideTooltipText();
		}
		if (!(item is BuildingInfo))
			return;
		BuildingInfo building = item as BuildingInfo;

		GameObject previewObj = Instantiate(building.previewPrefab, Vector3.up * -10000f, building.previewPrefab.transform.rotation);
		gridX = building.gridSize;
		gridY = building.gridSize;
		gridZ = building.gridSize;
		currentPreviewObj = previewObj;
		currentPlacingItem = item as BuildingInfo;
		placingStructure = true;
	}

	void AddAllItems() {
		int i = 0;
		foreach(ItemInfo item in saveManager.allItems.items) {
			SetItem(new WorldItem(item, 1), i);
			i++;
		}
	}

	public WorldItem[] GetInventory ()
    {
		return items;
    }

	public bool CheckRecipe(Recipe recipe) {
		if(player.ActiveSystemMenu()) {
			return false;
		}
		int[] inputAmounts = new int[recipe.inputs.Length];

		foreach(WorldItem invItem in items) {
			if(invItem.item) {
				int i = 0;
				foreach(WorldItem item in recipe.inputs) {
					if(invItem.item == item.item) {
						inputAmounts[i] += invItem.amount;
						break;
					}
					i++;
				}
			}
		}

		bool canCraft = true;
		for(int i = 0; i < inputAmounts.Length; i++) {
			if(!(inputAmounts[i] >= recipe.inputs[i].amount)) {
				canCraft = false;
			}
		}

		return canCraft;
	}

	public WorldItem GetHeldItem ()
    {
		return items[selectedHotbarSlot];
    }

	public void ClearInventory() {
		for(int i = 0; i < items.Length; i++) {
			items[i].Clear();
		}

		InventoryUpdate();
	}

	public void ConstructRecipe(Recipe recipe) {
		
		int[] inputAmounts = new int[recipe.inputs.Length];

		int o = 0; //COME BACK
		foreach(WorldItem invItem in items) {
			if(invItem.item) { // If the inventory slot has an item in it
				int i = 0;
				foreach(WorldItem item in recipe.inputs) { // Loop through all the ingredients in the recipe to see if the slot's item is the same as one of them
					if(invItem.item == item.item) { // Is the item the same?
						int amountToDecrease = Mathf.Max(0, item.amount - inputAmounts[i]);
						inputAmounts[i] += invItem.amount; // Add the amount of that item to the inputAmounts
						RemoveItem(o, amountToDecrease);
						break;
					}
					i++;
				}
			}
			o++;
		}

		AddItem(recipe.output);

		int r = 0;
		foreach(WorldItem replacementItem in recipe.replacedItems) {
			AddItem(replacementItem);
			r++;
		}
	}

	public void InventoryUpdate() {
		currentSelectedItem = items[selectedHotbarSlot].item;
		foreach(Transform craftingRecipeObj in craftingRecipeContainer) {
			craftingRecipeObj.GetComponent<RecipeListItem>().InventoryUpdate();
		}
	}

}