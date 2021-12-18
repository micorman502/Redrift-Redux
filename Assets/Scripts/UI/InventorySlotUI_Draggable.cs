using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// A UI representation of an InventorySlot (Credit to Rugbug Redfern for providing me this script for use in Redrift:Redux)
/// </summary>
public class InventorySlotUI_Draggable : InventorySlotUI, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] Animation hoverAnim;
	[SerializeField] GameObject previewPrefab;
	GameObject previewPrefabInstance;
	
	static InventorySlotUI_Draggable hover;
	bool dragging;

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		dragging = true;
		previewPrefabInstance = Instantiate(previewPrefab, transform);
		previewPrefabInstance.GetComponent<InventorySlotUI>().Initialize(slot);
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
			return;
		dragging = false;
		Destroy(previewPrefabInstance);
		
		if(hover && hover != this)
		{
			if(eventData.button == PointerEventData.InputButton.Left)
			{
				slot.Dump(hover.slot);
			}
			else
			{
				slot.Split(hover.slot);
			}
		}
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
	{
		hover = this;
		if (hoverAnim)
        {
			hoverAnim.Play();
        }
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		hover = null;
	}

	private void Update()
	{
		if (dragging)
		{
			previewPrefabInstance.transform.position = Mouse.current.position.ReadValue();
		}
	}
}
