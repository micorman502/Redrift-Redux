using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {

	[SerializeField] int hotbarSize;
	[SerializeField] GameObject placementParticleSystem;

	public GameObject inventoryContainer;


	[SerializeField] HeldItem[] heldItems;

	[SerializeField] int inventorySize;
	public Inventory inventory;
	[SerializeField] InventorySlot heldItemSlot;
	//[SerializeField] WorldItem[] items;

	PlayerController player;

	AudioManager audioManager;

	[HideInInspector] public bool placingStructure;

	public int selectedHotbarSlot = 0;
	public WorldItem currentSelectedItem;

	SaveManager saveManager;

	int mode;

	[SerializeField] int heldItemIndex = -1;
	int previousHeldItemIndex = -1;


	void Awake() {
		SetupNewInventory(inventorySize);

		audioManager = FindObjectOfType<AudioManager>();
		saveManager = FindObjectOfType<SaveManager>();

		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		//InventoryEvents.InitialiseInventoryUI(hotbarSize, items.Length);
	}

    private void Start()
    {
		InventoryUIManager.Instance.GetInventoryUI(InventoryUIManager.InventoryType.Primary).Assign(inventory);
		InventoryUpdate();
	}

	void SetupNewInventory (int inventorySize)
    {
		if (inventory != null)
        {
			inventory.ItemOverflow -= SimpleDropItem;
        }

		inventory = new Inventory(inventorySize);
		inventory.ItemOverflow += SimpleDropItem;
	}

    public void LoadCreativeMode() {
		mode = 1;

		//InventoryEvents.InitialiseInventoryUI(hotbarSize, items.Length);
		AddAllItems();
	}

	public void Pickup(ItemHandler itemHandler) {
		if (inventory.SpaceLeftForItem(new WorldItem(itemHandler.item, 1)) > 0)
		{
			inventory.AddItem(new WorldItem(itemHandler.item, 1));
			Destroy(itemHandler.gameObject);
		}
	}

	void Update() {

		for(int i = 0; i < hotbarSize; i++) {
			if (Input.GetKeyDown((i + 1).ToString()))
			{
				SetHotbarSlot(i, false);
			}
		}

		if((Input.GetAxisRaw("Mouse ScrollWheel") != 0 || Input.GetButtonDown("CycleRight") || Input.GetButtonDown("CycleLeft")) && !player.InMenu()) {
			if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 || Input.GetButtonDown("CycleLeft")) {
				ScrollHotbar(-1);
			} else {
				ScrollHotbar(1);
			}
		}
		CheckItemFunctions();


		if(!player.dead) {
			if(currentSelectedItem.item && !player.InMenu()) {
				if(Input.GetButtonDown("Drop")) {
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
		} 
		if (Input.GetKeyDown(KeyCode.R))
        {
			if (heldItemIndex != -1)
				heldItems[heldItemIndex].SpecialUse();
		}
		//the heldItemIndex check is done 4 times in case the heldItemIndex changes in the process of doing, say, the Use() functions.
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
			SetHotbarSlot(selectedHotbarSlot + scrollAmt, false);
        }
	}

	void SetHotbarSlot (int slot, bool allowNegativeOne) //allowNegativeOne maybe isn't the best name. setting it to true will allow selectedHotbarSlot to be -1.
    {
		selectedHotbarSlot = slot;
		if (selectedHotbarSlot < -1 && allowNegativeOne || selectedHotbarSlot < 0 && !allowNegativeOne)
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
	}

	void EquipItem (int slotIndex)
    {
		EquipItem(inventory.Slots[slotIndex]);
    }

	void EquipItem (InventorySlot slot)
    {
		if (heldItemSlot == slot)
        {
			slot = null;
			selectedHotbarSlot = -1;
		}
		if (heldItemSlot != null)
        {
			heldItemSlot.ItemChanged -= UpdateSelectedItem;
			heldItemSlot.CountChanged -= UpdateSelectedItem;
		}

		if (slot != null)
		{
			heldItemSlot = slot;
			currentSelectedItem = new WorldItem(heldItemSlot.Item, heldItemSlot.Count);
			heldItemSlot.ItemChanged += UpdateSelectedItem;
			heldItemSlot.CountChanged += UpdateSelectedItem;

			InventoryEvents.UpdateSelectedSlot(slot);
			EquipHeldItem(heldItemSlot.Item);
		} else
        {
			heldItemSlot.ItemChanged -= UpdateSelectedItem;
			heldItemSlot.CountChanged -= UpdateSelectedItem;
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

	void UpdateSelectedItem (ItemInfo item)
    {
		currentSelectedItem.item = item;
		EquipHeldItem(item);
		InventoryUpdate();
    }

	void UpdateSelectedItem (int amount)
    {
		currentSelectedItem.amount = amount;
		InventoryUpdate();
	}

	public void SimpleDropItem (WorldItem item)
    {
		SimpleDropItem(item.item, item.amount);
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

	public void DropItem (WorldItem item)
    {
		DropItem(item.item, item.amount);
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

	void AddAllItems() {
		//inventory.InventoryChanged -= InventoryUpdate;
		SetupNewInventory(saveManager.allItems.items.Length);
		//inventory.InventoryChanged += InventoryUpdate;
		InventoryUIManager.Instance.GetInventoryUI(InventoryUIManager.InventoryType.Primary).Assign(inventory);
		int i = 0;
		foreach(ItemInfo item in saveManager.allItems.items) {
			inventory.SetSlot(new WorldItem(item, item.stackSize), i);
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

	public void InventoryUpdate() {
		currentSelectedItem = new WorldItem(inventory.Slots[selectedHotbarSlot].Item, inventory.Slots[selectedHotbarSlot].Count);
	}

}