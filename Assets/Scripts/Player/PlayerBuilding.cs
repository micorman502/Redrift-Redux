using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Transform camTransform;
    [SerializeField] Transform buildingPlacementPoint;
    [SerializeField] ItemInfo buildingTemp;
    BuildingInfo currentBuilding;
    GameObject previewObject;
    int currentRotation;

    private void Update ()
    {
        if (IsBuilding())
        {
            AlignBuildingRot();
            AlignBuildingPos();
        }
    }

    void AlignBuildingPos ()
    {
        Vector3 placementPos = buildingPlacementPoint.transform.position;
        Vector3 pos;

        if (currentBuilding.gridSize <= 0)
        {
            pos = placementPos;
        }
        else
        {
            float grid = currentBuilding.gridSize * 4;
            pos = new Vector3(Mathf.Round(placementPos.x * grid) / grid, Mathf.Round(placementPos.y * grid) / grid, Mathf.Round(placementPos.z * grid) / grid);
        }

        if (currentBuilding.alignToNormal)
        {
            if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hitInfo, 5f, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore))
            {
                pos = hitInfo.point;
            }
        }

        previewObject.transform.position = pos + Vector3.Scale(previewObject.transform.up, currentBuilding.upwardsOffset);
    }

    public void AlignBuildingRot ()
    {
        if (!previewObject)
            return;

        Vector3 normal = Vector3.zero;
        if (currentBuilding.alignToNormal)
        {
            if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hitInfo, 5f, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore))
            {
                normal = hitInfo.normal;
            }
        }

        previewObject.transform.up = normal;

        previewObject.transform.rotation *= Quaternion.Euler(GetCurrentRotation());

        //previewObject.transform.eulerAngles = currentBuilding.possibleRotations[currentRotation];
    }

    public void StartBuilding (BuildingInfo building)
    {
        if (currentBuilding)
        {
            StopBuilding();
        }

        HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Rotate", KeyCode.R, HotTextInfo.Priority.Rotate, "buildingRotate"));

        currentBuilding = building;
        previewObject = Instantiate(building.previewPrefab, buildingPlacementPoint.position, Quaternion.identity);

        AlignBuildingRot();
        AlignBuildingPos();
    }

    public void StopBuilding ()
    {
        if (previewObject)
        {
            currentBuilding = null;
            Destroy(previewObject);
            HotTextManager.Instance.RemoveHotText("buildingRotate");
        }
    }

    public void PlaceBuilding ()
    {
        if (!LookLocker.MouseLocked)
            return;
        if (!BuildingValid())
            return;
        AlignBuildingRot();
        AlignBuildingPos();

        Instantiate(currentBuilding.placedObject, previewObject.transform.position, previewObject.transform.rotation);
        inventory.inventory.RemoveItem(new WorldItem(currentBuilding, 1));

        AudioManager.Instance.Play("Build");
    }

    bool BuildingValid ()
    {
        if (!currentBuilding)
            return false;
        if (inventory.inventory.GetItemTotal(currentBuilding) <= 0)
            return false;
        Physics.Linecast(camTransform.position, previewObject.transform.position, out RaycastHit hit, LayerMask.GetMask("World"), QueryTriggerInteraction.Ignore);
        if (hit.transform && Vector3.Distance(hit.point, previewObject.transform.position) > currentBuilding.approximateRadius)
            return false;

        return true;
    }

    public void SetBuildingRotation (int rotationChange)
    {
        if (!currentBuilding)
            return;

        currentRotation += rotationChange;
        if (currentRotation < 0)
        {
            currentRotation = currentBuilding.possibleRotations.Length;
        }
        else if (currentRotation >= currentBuilding.possibleRotations.Length)
        {
            currentRotation = 0;
        }
    }

    public bool IsBuilding ()
    {
        if (previewObject && currentBuilding)
        {
            return true;
        } else
        {
            return false;
        }
    }

    Vector3 GetCurrentRotation ()
    {
        return currentBuilding.possibleRotations[Mathf.Clamp(currentRotation, 0, currentBuilding.possibleRotations.Length - 1)];
    }
}
