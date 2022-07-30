using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldToolItem : HeldItem
{
    [SerializeField] PlayerController controller;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Animation anim;
    ToolInfo tool;
    bool usedThisFrame;
    float gatherLength;
    IResource currentResource;
    GameObject currentResourceObject;

    void Awake()
    {
        tool = item as ToolInfo;
    }

    public override void ItemUpdate()
    {
        if (usedThisFrame && currentResource != null)
        {
            gatherLength += Time.deltaTime;
            if (gatherLength >= currentResource.GetResource().gatherTime / tool.gatherSpeedMult)
            {
                gatherLength = 0;
                WorldItem[] gatheredItems = currentResource.ToolGather(tool);
                foreach (WorldItem gathered in gatheredItems)
                {
                    inventory.inventory.AddItem(gathered);
                }
            }
            UIEvents.CallUpdateProgressBar(gatherLength);
        } else
        {
            gatherLength = 0;
        }

        usedThisFrame = false;
    }

    public override void UseRepeating()
    {
        if (controller.GetTarget() == currentResourceObject && currentResourceObject != null)
        {
            usedThisFrame = true;
            return;
        }
        else
        {
            currentResource = null;
            currentResourceObject = null;
        }
        if (currentResource == null)
        {
            currentResource = controller.GetTarget()?.GetComponent<IResource>();
        }
        if (currentResource != null)
        {
            usedThisFrame = true;
            UIEvents.CallInitialiseProgressBar(currentResource.GetResource().gatherTime / tool.gatherSpeedMult);
        }
    }
}
