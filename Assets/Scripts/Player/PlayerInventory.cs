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

	[SerializeField] int inventorySize;
	public Inventory inventory;
	[SerializeField] InventorySlot heldItemSlot;
	//[SerializeField] WorldItem[] items;

	PlayerController player;

	AudioManager audioManager;

	[HideInInspector] public bool placingStructure;
	GameObject currentPreviewObj;
	BuildingInfo currentPlacingItem;
	int currentPlacingRot;

	public int selectedHotbarSlot = 0;
	public WorldItem currentSelectedItem;

	SaveManager saveManager;

	int mode;

	float gridX = 1f;
	float gridY = 0.25f;
	float gridZ = 1f;

	[SerializeField] int heldItemIndex = -1;
	int previousHeldItemIndex = -1;


	void Awake() {
		inventory = new Inventory(inventorySize);
		audioManager = FindObjectOfType<AudioManager>();
		saveManager = FindObjectOfType<SaveManager>();

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		//InventoryEvents.InitialiseInventoryUI(hotbarSize, items.Length);
	}

    private void Start()
    {
		InventoryUIManager.Instance.GetInventoryUI(InventoryUIManager.InventoryType.Primary).Assign(inventory);
    }

    public void LoadCreativeMode() {
		mode = 1;

		//InventoryEvents.InitialiseInventoryUI(hotbarSize, items.Length);
		AddAllItems();

		craftingContainer.SetActive(false);
	}

	public void Pickup(ItemHandler itemHandler) {
		if(mode != 1) {
			inventory.AddItem(new WorldItem(itemHandler.item, 1)); //TODO: Check if inventory is full first!
		}
		Destroy(itemHandler.gameObject);
	}

	void Update() {

		for(int i = 0; i < hotbarSize; i++) {
			if (Input.GetKeyDown((i + 1).ToString()))
			{
				SetHotbarSlot(i);
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

				if (currentSelectedItem.item is BuildingInfo)
				{
					BuildingInfo building = currentSelectedItem.item as BuildingInfo;
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
			if(currentSelectedItem.item && !player.InMenu()) {
				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					if(currentSelectedItem.item is BuildingInfo && !placingStructure) {
						StartBuilding(currentSelectedItem.item);
					}

					InventoryUpdate();
				} else if(Input.GetButtonDown("Drop")) {
					if(Input.GetButton("Supersize")) {
						if(mode != 1) {
							DropItem(currentSelectedItem.item, Ping(5, inventory.GetItemTotal(currentSelectedItem.item)));
						} else {
							DropItem(currentSelectedItem.item, 5);
						}
					} else {
						DropItem(currentSelectedItem.item, 1);
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
		selectedHotbarSlot = slot;
		if (selectedHotbarSlot < -1)
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
		if (selectedHotbarSlot != -1)
		{
			EquipItem(selectedHotbarSlot);
		} else
        {
			EquipItem(null);
        }
		if (placingStructure)
		{
			StopBuilding();
		}
	}

	void EquipItem (int slotIndex)
    {
		EquipItem(inventory.Slots[slotIndex]);
    }

	void EquipItem (InventorySlot slot)
    {
		if (heldItemSlot == slot && slot != null)
        {
			slot = null;
			selectedHotbarSlot = -1;
        }

		if (heldItemSlot != null)
        {
			heldItemSlot.ItemChanged -= EquipHeldItem;
		}

		if (slot != null)
		{
			heldItemSlot = slot;
			currentSelectedItem = new WorldItem(heldItemSlot.Item, heldItemSlot.Count);
			heldItemSlot.ItemChanged += EquipHeldItem;
			InventoryEvents.UpdateSelectedSlot(slot);
			EquipHeldItem(heldItemSlot.Item);
		} else
        {
			heldItemSlot = null;
			currentSelectedItem = new WorldItem();
			InventoryEvents.UpdateSelectedSlot(slot);
			EquipHeldItem(null);
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

		if (selectedHotbarSlot != -1)
		{
			heldItemSlot = inventory.Slots[selectedHotbarSlot];
		} else
        {
			heldItemSlot = null;
        }

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
		inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
		amount = amtTaken;
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
		inventory.RemoveItem(new WorldItem(currentPlacingItem, 1));
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
		inventory = new Inventory(saveManager.allItems.items.Length);
		InventoryUIManager.Instance.GetInventoryUI(InventoryUIManager.InventoryType.Primary).Assign(inventory);
		int i = 0;
		foreach(ItemInfo item in saveManager.allItems.items) {
			inventory.SetSlot(new WorldItem(item, 10000), i);
			i++;
		}
	}

	public bool CheckRecipe(Recipe recipe) {
		if(player.ActiveSystemMenu()) {
			return false;
		}
		int[] inputAmounts = new int[recipe.inputs.Length];

		foreach(InventorySlot invItem in inventory.Slots) {
			if(invItem.Item) {
				int i = 0;
				foreach(WorldItem item in recipe.inputs) {
					if(invItem.Item == item.item) {
						inputAmounts[i] += invItem.Count;
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
		return currentSelectedItem;
    }

	public void ClearInventory() {
		for(int i = 0; i < inventory.Slots.Length; i++) {
			inventory.Slots[i].Clear();
		}

		InventoryUpdate();
	}

	public void ConstructRecipe(Recipe recipe) {
		
		int[] inputAmounts = new int[recipe.inputs.Length];

		int o = 0; //COME BACK
		foreach(InventorySlot invItem in inventory.Slots) {
			if(invItem.Item) { // If the inventory slot has an item in it
				int i = 0;
				foreach(WorldItem item in recipe.inputs) { // Loop through all the ingredients in the recipe to see if the slot's item is the same as one of them
					if(invItem.Item == item.item) { // Is the item the same?
						int amountToDecrease = Mathf.Max(0, item.amount - inputAmounts[i]);
						inputAmounts[i] += invItem.Count; // Add the amount of that item to the inputAmounts
						inventory.RemoveItem(new WorldItem(item.item, amountToDecrease));
						break;
					}
					i++;
				}
			}
			o++;
		}

		inventory.AddItem(recipe.output);

		int r = 0;
		foreach(WorldItem replacementItem in recipe.replacedItems) {
			inventory.AddItem(replacementItem);
			r++;
		}
	}

	public void InventoryUpdate() {
		currentSelectedItem = new WorldItem(inventory.Slots[selectedHotbarSlot].Item, inventory.Slots[selectedHotbarSlot].Count);
		foreach(Transform craftingRecipeObj in craftingRecipeContainer) {
			craftingRecipeObj.GetComponent<RecipeListItem>().InventoryUpdate();
		}
	}

}