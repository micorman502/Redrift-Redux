using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldItem
{
    public ItemInfo item;
    public int amount;

    public void Clear ()
    {
        item = null;
        amount = 0;
    }

    public bool AtMaxStack ()
    {
        if (amount >= item.stackSize)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public WorldItem (ItemInfo _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }

    public WorldItem ()
    {
        item = null;
        amount = 0;
    }
}
