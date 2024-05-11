using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Other/Weapon")]
public class Weapon : ScriptableObject
{
    public enum WeaponType { Melee, Bow };
    public WeaponType type;
    public float damage;
    public GameObject arrowPrefab;
    public Transform arrowSpawn;
    public float chargeTime;
}
