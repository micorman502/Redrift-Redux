using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] Transform camPoint;
    [SerializeField] PlayerInventory inventory;
    DeployableInfo deployable;

    void Start ()
    {
        deployable = (DeployableInfo)item;
        if (!camPoint)
        {
            camPoint = Camera.main.transform;
        }
    }

    public override void Use ()
    {
        Deploy();
    }

    void Deploy ()
    {
        if (inventory.inventory.GetItemTotal(item) <= 0)
            return;
        if (!SpawnValid(out Vector3 spawnPos))
            return;

        inventory.inventory.RemoveItem(item);
        Instantiate(deployable.deployedObject, spawnPos, Quaternion.identity);
    }

    void FixedUpdate ()
    {
        if (!itemGameObject.activeInHierarchy)
            return;

        if (SpawnValid(out Vector3 dummy))
        {
            HotTextManager.Instance.UpdateHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", false));
        }
        else
        {
            HotTextManager.Instance.UpdateHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", true));
        }
    }

    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            if (SpawnValid(out Vector3 dummy))
            {
                HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", false));
            }
            else
            {
                HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", true));
            }
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("deployable");
        }
    }

    bool SpawnValid (out Vector3 spawnPos)
    {
        spawnPos = Vector3.zero;
        bool hitSuccessful = Physics.Raycast(camPoint.position, camPoint.forward, out RaycastHit hit, PlayerController.interactRange, ~LayerMask.GetMask("Ignore Raycast", "Block Building"), QueryTriggerInteraction.Collide);

        if (!hitSuccessful)
        {
            spawnPos = camPoint.position + camPoint.forward * 2.5f;

            if (deployable.placeableMidAir)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        GameObject physParent = hit.rigidbody != null ? hit.rigidbody.gameObject : hit.collider.gameObject;

        spawnPos = hit.point + deployable.basePlacementOffset + (hit.normal * deployable.normalPlacementOffset);

        if (deployable.placeableGround && !hit.collider.isTrigger)
        {
            return true;
        }

        if (deployable.placeableWater && hit.collider.isTrigger && physParent.layer == LayerMask.NameToLayer("Water"))
        {
            return true;
        }

        return false;
    }
}
