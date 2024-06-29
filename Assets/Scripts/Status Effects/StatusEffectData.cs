using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Status Effect Data")]
public class StatusEffectData : ScriptableObject
{
    public StatusEffect[] statusEffects;
}

[System.Serializable]
public class StatusEffect
{
    public string accessor;
    public Sprite icon;
    public GameObject statusEffectObject;
    public bool buff;
    public bool immutable;
    public bool stackable;
    public int maxStack;
    public float effectIntensity;
    public int id;
}
