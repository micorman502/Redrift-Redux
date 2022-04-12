using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilding : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Transform buildingPlacementPoint;
    [SerializeField] ItemInfo buildingTemp;
    BuildingInfo currentBuilding;
    GameObject previewObject;
    bool inMenu;
    int currentRotation;

    void OnEnable()
    {
        ControlEvents.OnLockStateSet += SetInMenuState;
    }

    void OnDisable()
    {
        ControlEvents.OnLockStateSet -= SetInMenuState;
    }

    void SetInMenuState (bool state)
    {
        inMenu = !state;
    }

    private void Update ()
    {
        if (IsBuilding())
        {
            AlignBuilding();
            NoticeTextManager.Instance.AddNoticeText("[LMB] to place, [R] to rotate", 2);
        }
    }

    void AlignBuilding ()
    {
        Vector3 pos = buildingPlacementPoint.transform.position;
        float grid = currentBuilding.gridSize * 4;
        previewObject.transform.position = new Vector3(Mathf.Round(pos.x * grid) / grid, Mathf.Round(pos.y * grid) / grid, Mathf.Round(pos.z * grid) / grid);
    }

    public void StartBuilding (BuildingInfo building)
    {
        if (currentBuilding)
        {
            StopBuilding();
        }

        currentBuilding = building;
        previewObject = Instantiate(building.previewPrefab, buildingPlacementPoint.position, Quaternion.identity);
        AlignBuilding();
        SetBuildingRotation(currentRotation);
    }

    public void StopBuilding ()
    {
        if (previewObject)
        {
            currentBuilding = null;
            Destroy(previewObject);
        }
    }

    public void PlaceBuilding ()
    {
        if (inMenu)
            return;
        if (inventory.inventory.GetItemTotal(currentBuilding) > 0)
        {
            Instantiate(currentBuilding.placedObject, previewObject.transform.position, Quaternion.Euler(currentBuilding.possibleRotations[currentRotation]));
            AlignBuilding();
            inventory.inventory.RemoveItem(new WorldItem(currentBuilding, 1));
        }
    }

    public void RotateBuilding (int rotationChange)
    {
        SetBuildingRotation(currentRotation + rotationChange);
    }

    public void SetBuildingRotation (int rotationAmount)
    {
        if (!previewObject)
            return;
        if (rotationAmount < 0)
        {
            rotationAmount = currentBuilding.possibleRotations.Length;
        } else if (rotationAmount >= currentBuilding.possibleRotations.Length)
        {
            rotationAmount = 0;
        }
        currentRotation = rotationAmount;

        previewObject.transform.eulerAngles = currentBuilding.possibleRotations[currentRotation];
    }

    public bool IsBuilding ()
    {
        if (previewObject)
        {
            return true;
        } else
        {
            return false;
        }
    }
}
