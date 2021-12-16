﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItem : MonoBehaviour, IItemSaveable, IInteractable {

	[SerializeField] ItemHandler handler;
	[SerializeField] int saveID;
	public float[] intensities = { 0f, 0.25f, 0.5f, 1f };
	public int intensityNum = 0;

	public bool active = true;

	new Light light;

	void Awake() {
		light = GetComponentInChildren<Light>();
	}

	void Start() {
		UpdateActive();
		UpdateIntensity();
	}

	public void Interact ()
    {
		IncreaseIntensity();
    }

	public void SetIntensity(int newIntensityNum) {
		intensityNum = newIntensityNum;
		if (intensityNum >= intensities.Length)
        {
			intensityNum = 0;
        }

		if(intensities[intensityNum] == 0) {
			active = false;
			UpdateActive();
		} else if(!active) {
			active = true;
			UpdateActive();
		}
		UpdateIntensity();
	}

	public void IncreaseIntensity() {
		SetIntensity(intensityNum + 1);
	}

	void UpdateActive() {
		light.enabled = active;
		UpdateTooltip();
	}

	void UpdateIntensity() {
		light.intensity = intensities[intensityNum];
		UpdateTooltip();
	}

	void UpdateTooltip ()
    {
		if (active)
		{
			handler.SetNoticeText("Hold [E] to pick up, [F] to adjust brightness");
		}
		else
		{
			handler.SetNoticeText("Hold [E] to pick up, [F] to turn on");
		}
	}

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, saveID);

		newData.num = intensityNum;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		SetIntensity(data.num);
	}
}
