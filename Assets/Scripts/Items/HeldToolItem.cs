using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldToolItem : HeldItem
{
    [SerializeField] PlayerController controller;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Animation anim;
    ToolInfo tool;
    bool keyPressedThisFrame;
    float gatherStartedTime;
    float gatherDuration;
    float gatherLength;
    bool gathering;

    IResource currentResource;
    GameObject currentResourceObject;

    void Awake()
    {
        tool = item as ToolInfo;
    }

    public override void ItemUpdate ()
    {
        if (gathering)
        {
            if (keyPressedThisFrame)
            {
                ProgressGather();
            } else
            {
                StopGather();
            }
        } else
        {
            if (keyPressedThisFrame)
            {
                StartGather(controller.GetTarget());
            }
        }

        keyPressedThisFrame = false;
    }

    void StopGather ()
    {
        if (!gathering)
            return;
        gathering = false;

        currentResourceObject = null;
        currentResource = null;

        UIEvents.CallDisableProgressBar();
    }

    void StartGather (GameObject target)
    {
        if (gathering)
            return;
        if (!target)
            return;

        IResource resource = target.GetComponent<IResource>();
        if (resource == null)
        {
            UIEvents.CallProgressBarFail();
            return;
        }

        gathering = true;
        gatherStartedTime = Time.time;
        gatherDuration = resource.GetResource().gatherTime / tool.gatherSpeedMult;

        currentResourceObject = target;
        currentResource = resource;

        UIEvents.CallInitialiseProgressBar(gatherDuration);
    }

    void ProgressGather ()
    {
        if (!gathering)
            return;
        if (!currentResourceObject)
        {
            StopGather();
            return;
        }
        GameObject controllerTarget = controller.GetTarget();
        if (!controllerTarget || controllerTarget != currentResourceObject && controllerTarget.transform.parent != currentResourceObject.transform.parent)
        {
            StopGather();
            return;
        }

        float progression = Time.time - gatherStartedTime;

        UIEvents.CallUpdateProgressBar(progression);

        if (progression > gatherDuration)
        {
            FinishGather();
        }
    }

    void FinishGather ()
    {
        if (!gathering)
            return;

        WorldItem[] gatheredItems = currentResource.ToolGather(tool);
        foreach (WorldItem gathered in gatheredItems)
        {
            inventory.inventory.AddItem(gathered);
        }

        StopGather();
    }

    public override void UseRepeating()
    {
        keyPressedThisFrame = true;
    }
}
