using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public const bool itemDebugMode = true;
    public static ItemDatabase Instance { get; private set; }
    static ItemRegister Register;
    [SerializeField] ItemRegister register;

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already an ItemDatabase in existence. Deleting this ItemDatabase.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Register = register;

        CompileItems();

        if (itemDebugMode)
        {
            DebugItems();
        }
    }

    static void CompileItems ()
    {
        for (int i = 0; i < Register.items.Length; i++)
        {
            Register.items[i].id = i;
            Register.items[i].CompileDescription();
        }
    }

    void DebugItems ()
    {
        for (int i = 0; i < register.items.Length; i++)
        {
            ItemInfo item = register.items[i];
            string baseInfo = $"Item '{item.itemName}' Id #{item.id}";
            if (item.droppedPrefab == null)
            {
                Debug.LogWarning(baseInfo + " is missing its droppedPrefab");
            }
            if (item.icon == null)
            {
                Debug.LogWarning(baseInfo + " is missing its icon");
            }

            BuildingInfo building = item as BuildingInfo;

            if (building)
            {
                if (building.previewPrefab == null)
                {
                    Debug.LogWarning(baseInfo + " is missing its previewPrefab");
                }
                if (building.placedObject == null)
                {
                    Debug.LogWarning(baseInfo + " is missing its placedObject");
                }
            }
        }
    }

    public static ItemInfo[] GetAllItems ()
    {
        return Register.items;
    }

    public static ItemInfo GetItem (int id)
    {
        return Register.items[id];
    }

    public static ItemInfo GetItemByInternalName (string objectName)
    {
        for (int i = 0; i < Register.items.Length; i++)
        {
            if (Register.items[i].name == objectName)
            {
                return Register.items[i];
            }
        }

        Debug.Log("No item with the object name '" + objectName + "' could be found. Returning first item.");
        return Register.items[0];
    }

    public static ItemInfo GetItemByExternalName (string itemName)
    {
        for (int i = 0; i < Register.items.Length; i++)
        {
            if (Register.items[i].itemName == itemName)
            {
                return Register.items[i];
            }
        }

        Debug.Log("No item with the name '" + itemName + "' could be found. Returning first item.");
        return Register.items[0];
    }
}
