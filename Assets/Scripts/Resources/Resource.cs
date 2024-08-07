﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Other/Resource")]
public class Resource : ScriptableObject
{
    public string resourceName;
    public GameObject prefab;
    public int id;
    public ItemInfo[] resourceItems;
    public float[] chances;
    public int maxGathers;
    public float gatherTime;
    public bool infiniteGathers = false;
}
