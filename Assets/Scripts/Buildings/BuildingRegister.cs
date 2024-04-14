using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RollingTundra.DataUtils;

[CreateAssetMenu(menuName = "Registers/Building Register")]
public class BuildingRegister : ScriptableObject
{
    public bool autoFindBuildings;
    public BuildingData[] buildings;
    public Recipe[] buildingRecipes;

    public void SearchBuildings ()
    {
        if (!autoFindBuildings)
            return;

        if (!Application.isEditor)
            return;

        RegisterUtils.PopulateRegister(this, ref buildings);
        
    }
}
