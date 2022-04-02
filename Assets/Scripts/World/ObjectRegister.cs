using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectRegister : ScriptableObject
{
    public ObjectRegisterObject[] objects;
}

[System.Serializable]
public struct ObjectRegisterObject
{
    public GameObject prefab;
    public string nameIdentifier;
    public int id;
}
