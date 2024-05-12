using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Transform camTransform;
    [SerializeField] Transform buildingPlacementPoint;
    BuildingInfo currentBuilding;
    Recipe currentBuildingRecipe;
    BuildingPreview preview;
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

        preview.transform.position = pos + Vector3.Scale(preview.transform.up, currentBuilding.upwardsOffset);
    }

    public void AlignBuildingRot ()
    {
        if (!preview)
            return;

        Vector3 normal = Vector3.zero;
        if (currentBuilding.alignToNormal)
        {
            if (Physics.Raycast(camTransform.position, camTransform.forward, out RaycastHit hitInfo, 5f, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore))
            {
                normal = hitInfo.normal;
            }
        }

        preview.transform.up = normal;

        preview.transform.rotation *= Quaternion.Euler(GetCurrentRotation());

        //previewObject.transform.eulerAngles = currentBuilding.possibleRotations[currentRotation];
    }

    public void StartBuilding (BuildingInfo building, Recipe buildingRecipe = null)
    {
        if (currentBuilding)
        {
            StopBuilding();
        }

        HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Rotate", KeyCode.R, HotTextInfo.Priority.Rotate, "buildingRotate"));

        currentBuilding = building;
        currentBuildingRecipe = buildingRecipe;
        preview = Instantiate(building.previewPrefab, buildingPlacementPoint.position, Quaternion.identity).GetComponent<BuildingPreview>();

        AlignBuildingRot();
        AlignBuildingPos();
    }

    public void StopBuilding ()
    {
        if (!preview)
            return;

        currentBuilding = null;
        Destroy(preview.gameObject);

        HotTextManager.Instance.RemoveHotText("buildingRotate");
    }

    public void PlaceBuilding ()
    {
        if (!LookLocker.MouseLocked)
            return;

        if (!BuildingValid())
            return;
        if (!ItemCheck())
            return;

        AlignBuildingRot();
        AlignBuildingPos();

        Instantiate(currentBuilding.placedObject, preview.transform.position, preview.transform.rotation);

        AudioManager.Instance.Play("Build");
    }

    bool BuildingValid ()
    {
        if (!currentBuilding)
            return false;

        if (!preview.IsValid())
            return false;

        Physics.Linecast(camTransform.position, preview.transform.position, out RaycastHit hit, LayerMask.GetMask("World"), QueryTriggerInteraction.Ignore);
        if (hit.transform && Vector3.Distance(hit.point, preview.transform.position) > currentBuilding.approximateRadius)
            return false;

        return true;
    }

    bool ItemCheck ()
    {
        if (inventory.inventory.RemoveItem(currentBuilding, 1) > 0)
            return true;

        if (currentBuildingRecipe && currentBuildingRecipe.Craft(inventory.inventory, true, false))
            return true;

        return false;
    }

    public void SetBuildingRotation (int rotationChange)
    {
        if (!currentBuilding)
            return;

        currentRotation = Mathf.RoundToInt(Mathf.Repeat(currentRotation + rotationChange, currentBuilding.possibleRotations.Length));
    }

    public bool IsBuilding ()
    {
        if (preview && currentBuilding)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 GetCurrentRotation ()
    {
        return currentBuilding.possibleRotations[Mathf.Clamp(currentRotation, 0, currentBuilding.possibleRotations.Length - 1)];
    }
}