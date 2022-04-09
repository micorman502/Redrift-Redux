using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Registers/Item Register")]
public class ItemRegister : ScriptableObject
{
    public ItemInfo[] items;
}
