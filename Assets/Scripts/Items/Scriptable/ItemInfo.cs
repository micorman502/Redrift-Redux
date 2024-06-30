using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Base", order = 0)]
public class ItemInfo : ScriptableObject
{
    public string itemName;
    [TextArea(2, 5)] public string itemDescription;
    public int stackSize = 100;
    public string stackPrefix = "x";
    public int id;
    public int achievementId = -1;
    public Sprite icon;
    public GameObject droppedPrefab;

    public virtual void CompileDescription ()
    {

    }

    protected void AssignDescriptionStat (float stat, string statKey)
    {
        AssignDescriptionStat(stat.ToString(), statKey);
    }

    protected void AssignDescriptionStat (int stat, string statKey)
    {
        AssignDescriptionStat(stat.ToString(), statKey);
    }

    protected void AssignDescriptionStat (string stat, string statKey)
    {
        statKey = "{" + statKey + "}";
        itemDescription = itemDescription.Replace(statKey, stat);
    }
}
