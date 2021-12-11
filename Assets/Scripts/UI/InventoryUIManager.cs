using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public enum InventoryType { Primary, Storage };
    [SerializeField] InventoryType[] types;
    [SerializeField] InventoryUI[] inventoryUis;
    public static InventoryUIManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public InventoryUI GetInventoryUI (InventoryType type)
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i] == type)
            {
                return inventoryUis[i];
            }
        }
        Debug.LogError("InventoryUIManager.GetInventoryUI found no InventoryUI when searching for type: " + type);
        return null;
    }
}
