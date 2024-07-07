using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Status Effector")]
public class SEItemInfo : ItemInfo
{
    public string statusEffect;
    public float effectDuration;
    public int effectStacks = 1;
}
