using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Screenshot Data")]
public class ItemScreenshotData : ScriptableObject
{
    public int height = 512;
    public int width = 512;
    public bool useDefaultPath = true;
    public string customPath = "/_Items/Icons/";
}
