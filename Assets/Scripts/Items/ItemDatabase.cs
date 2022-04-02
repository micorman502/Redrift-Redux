using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
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
    }

    void IDItems ()
    {
        for (int i = 0; i < register.items.Length; i++)
        {
            register.items[i].id = i;
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
}
