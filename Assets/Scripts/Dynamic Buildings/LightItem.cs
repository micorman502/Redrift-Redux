using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItem : MonoBehaviour, IItemSaveable, IInteractable, IHotText {

	[SerializeField] ItemHandler handler;
	[SerializeField] Light pointLight;
	[SerializeField] string saveID;
	[SerializeField] float intensity;
	[SerializeField] float intensityVariation;
	[SerializeField] float intensityVariationSpeed;
	bool lightOn;

	void Start() {
		SetState(lightOn); //just to make sure the light is enabled/disabled properly when it's a new game
	}

	public void Interact ()
    {
		Toggle();
    }

	public void Toggle ()
    {
		SetState(!lightOn);
    }

	void Update ()
    {
		if (intensityVariation <= 0)
			return;

		pointLight.intensity = intensity + ((Mathf.PerlinNoise(Time.time * intensityVariationSpeed, Time.time * intensityVariationSpeed) * intensityVariation * 2) - intensityVariation);
    }

	public void SetState (bool on)
    {
		lightOn = on;
		if (lightOn)
        {
			pointLight.enabled = true;
			pointLight.intensity = intensity;
        } else
        {
			pointLight.enabled = false;
        }
    }

	public void GetData(out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
	{
		ItemSaveData newData = new ItemSaveData();
		ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

		newData.boolVal = lightOn;

		data = newData;
		objData = newObjData;
		dontSave = false;
	}

	public void SetData(ItemSaveData data, ObjectSaveData objData)
	{
		transform.position = objData.position;
		transform.rotation = objData.rotation;

		SetState(lightOn);
	}

	void IHotText.HideHotText ()
	{
		HotTextManager.Instance.RemoveHotText(new HotTextInfo(lightOn ? "Turn Off" : "Turn On", KeyCode.F, HotTextInfo.Priority.Interact, "lightToggle"));
	}

	void IHotText.ShowHotText ()
	{
		HotTextManager.Instance.ReplaceHotText(new HotTextInfo(lightOn ? "Turn Off" : "Turn On", KeyCode.F, HotTextInfo.Priority.Interact, "lightToggle"));
	}

	void IHotText.UpdateHotText ()
	{
		HotTextManager.Instance.UpdateHotText(new HotTextInfo(lightOn ? "Turn Off" : "Turn On", KeyCode.F, HotTextInfo.Priority.Interact, "lightToggle"));
	}
}
