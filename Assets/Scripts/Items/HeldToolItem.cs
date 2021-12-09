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
    ResourceHandler currentResource;
    GameObject currentResourceObject;

    void Awake()
    {
        tool = item as ToolInfo;
    }

    public override void ItemUpdate()
    {
        if (usedThisFrame && currentResource)
        {
            Debug.Log("valid");
            gatherLength += Time.deltaTime;
            if (gatherLength >= currentResource.resource.gatherTime / tool.gatherSpeedMult)
            {
                Debug.Log("is above");
                gatherLength = 0;
                WorldItem[] gatheredItems = currentResource.ToolGather(tool);
                foreach (WorldItem gathered in gatheredItems)
                {
                    inventory.AddItem(gathered);
                }
            }
        } else
        {
            Debug.Log("invalid");
            gatherLength = 0;
        }

        usedThisFrame = false;
    }

    public override void UseRepeating()
    {
        if (controller.target == currentResourceObject)
        {
            usedThisFrame = true;
            return;
        }
        else
        {
            currentResource = null;
            currentResourceObject = null;
        }
        if (!currentResource)
        {
            currentResource = controller.target.GetComponent<ResourceHandler>();
        }
        if (currentResource)
        {
            usedThisFrame = true;
        }
    }
}
