using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDatabase : MonoBehaviour
{
    public const bool buildingDebugMode = true;
    public static BuildingDatabase Instance { get; private set; }
    static BuildingRegister Register;
    [SerializeField] BuildingRegister register;

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already an BuldingDatabase in existence. Deleting this BuildingDatabase.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Register = register;

        Register.SearchBuildings();

        if (buildingDebugMode)
        {
            DebugBuildings();
        }
    }

    void DebugBuildings ()
    {
        for (int i = 0; i < register.buildings.Length; i++)
        {

            BuildingInfo building = register.buildings[i];

            if (building)
            {
                if (building.previewPrefab == null)
                {
                    Debug.LogWarning(building.itemName + " is missing its previewPrefab");
                }
                if (building.placedObject == null)
                {
                    Debug.LogWarning(building.itemName + " is missing its placedObject");
                }
            }
        }
    }

    public static BuildingInfo[] GetAllBuildings ()
    {
        return Register.buildings;
    }

    public static BuildingInfo GetBuilding (int id)
    {
        return Register.buildings[id];
    }

    public static BuildingInfo GetBuildingByInternalName (string objectName)
    {
        for (int i = 0; i < Register.buildings.Length; i++)
        {
            if (Register.buildings[i].name == objectName)
            {
                return Register.buildings[i];
            }
        }

        Debug.Log("No building with the object name '" + objectName + "' could be found. Returning first building.");
        return Register.buildings[0];
    }

    public static BuildingInfo GetBuildingByExternalName (string buildingName)
    {
        for (int i = 0; i < Register.buildings.Length; i++)
        {
            if (Register.buildings[i].itemName == buildingName)
            {
                return Register.buildings[i];
            }
        }

        Debug.Log("No building with the name '" + buildingName + "' could be found. Returning first building.");
        return Register.buildings[0];
    }

    public static Recipe[] GetAllBuildingRecipes ()
    {
        return Register.buildingRecipes;
    }
}
