﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevControls : MonoBehaviour {

	PlayerInventory inventory;

	SaveManager saveManager;

	void Awake() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
		saveManager = GameObject.FindGameObjectWithTag("ScriptHolder").GetComponent<SaveManager>();
	}

	void Update() {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I)) {
			AddAllItems();
		}
	}

	void AddAllItems() {
		foreach(ItemInfo item in saveManager.allItems.items) {
			inventory.AddItem(item, 1);
		}
	}
}
