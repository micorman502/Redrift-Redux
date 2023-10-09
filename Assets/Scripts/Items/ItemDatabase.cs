using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public const bool itemDebugMode = true;
    public static ItemDatabase Instance { get; private set; }
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

        IDItems();

        if (itemDebugMode)
        {
            DebugItems();
        }
    }

    void IDItems ()
    {
        for (int i = 0; i < register.items.Length; i++)
        {
            register.items[i].id = i;
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

    public ItemInfo[] GetAllItems ()
    {
        return register.items;
    }

    public ItemInfo GetItem (int id)
    {
        return register.items[id];
    }

    public ItemInfo GetItemByInternalName (string objectName)
    {
        for (int i = 0; i < register.items.Length; i++)
        {
            if (register.items[i].name == objectName)
            {
                return register.items[i];
            }
        }

        Debug.Log("No item with the object name '" + objectName + "' could be found. Returning first item.");
        return register.items[0];
    }

    public ItemInfo GetItemByExternalName (string itemName)
    {
        for (int i = 0; i < register.items.Length; i++)
        {
            if (register.items[i].itemName == itemName)
            {
                return register.items[i];
            }
        }

        Debug.Log("No item with the name '" + itemName + "' could be found. Returning first item.");
        return register.items[0];
    }
}
