using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : ItemHandler, IDamageable, IFlammable
{
    [SerializeField] float maxHealth;
    [SerializeField] bool flammable;
    [SerializeField] float flameReductionSpeed;
    [SerializeField] GameObject flameFxPrefab;
    GameObject currentFlameFx;
    float health;
    float flameStrength;

    void Awake ()
    {
        health = maxHealth;
    }

    public void Ignite (float ignitionStrength)
    {
        if (ignitionStrength <= 0)
            return;

        if (!flammable)
            return;

        if (flameStrength == 0)
        {
            currentFlameFx = Instantiate(flameFxPrefab, transform);

            BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider)
            {
                currentFlameFx.transform.localScale = boxCollider.size;
            }
        }

        flameStrength += ignitionStrength;
    }

    public void Extinguish (float extinguishStrength)
    {
        if (extinguishStrength <= 0)
            return;

        flameStrength -= extinguishStrength;

        if (flameStrength < 0)
        {
            flameStrength = 0;
        }

        if (flameStrength == 0 && currentFlameFx)
        {
            Destroy(currentFlameFx);
        }
    }

    public void FixedUpdate ()
    {
        if (!flammable || flameStrength == 0)
            return;

        RemoveHealth(flameStrength * Time.fixedDeltaTime);
        Extinguish(flameReductionSpeed * Time.fixedDeltaTime);
    }

    public void RemoveHealth (float healthRemoved)
    {
        health -= healthRemoved;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die ()
    {
        Destroy(gameObject);
    }

    public override void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
    {
        base.GetData(out data, out objData, out _dontSave);

        data = new ItemSaveData();
        data.floatVal = health;
        data.num = Mathf.CeilToInt(flameStrength);
    }

    protected override void Load (ItemSaveData data, ObjectSaveData objData)
    {
        base.Load(data, objData);

        Ignite(data.num);
        health = data.floatVal;
    }
}
