using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDatabase : MonoBehaviour
{
    public static StatusEffectDatabase Instance;
    public static StatusEffectData statusEffectData { get; private set; }
    [SerializeField] StatusEffectData targetStatusEffectData;

    public void Awake ()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        statusEffectData = targetStatusEffectData;

        for (int i = 0; i < statusEffectData.statusEffects.Length; i++)
        {
            statusEffectData.statusEffects[i].id = i;
        }
    }

    public static StatusEffect GetStatusEffect (string name)
    {
        for (int i = 0; i < statusEffectData.statusEffects.Length; i++)
        {
            if (statusEffectData.statusEffects[i].accessor == name)
            {
                return statusEffectData.statusEffects[i];
            }
        }
        Debug.LogWarning("Status Effect '" + name + "' could not be found. Returning new StatusEffect.");
        return new StatusEffect();
    }

    public static StatusEffect GetStatusEffect (int id)
    {
        if (id >= statusEffectData.statusEffects.Length)
        {
            Debug.LogWarning("Status Effect #" + id + " does not exist. Returning new StatusEffect.");
            return new StatusEffect();
        }
        return statusEffectData.statusEffects[id];
    }
}
