using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandlerFading : ItemHandler
{
    [SerializeField] float fadeTime = 5f;
    [SerializeField] Vector3 minScale = Vector3.one * 0.1f;
    Vector3 baseScale;
    float currentFade;

    void Awake ()
    {
        baseScale = transform.localScale;
        if (loaded)
            return;

        currentFade = 1;
    }

    public override void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
    {
        base.GetData(out data, out objData, out _dontSave);

        data = new ItemSaveData();
        data.floatVal = currentFade;
    }

    void FixedUpdate ()
    {
        currentFade -= (1 / fadeTime) * Time.fixedDeltaTime;

        if (currentFade <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.localScale = minScale + baseScale * currentFade;
    }

    protected override void Load (ItemSaveData data, ObjectSaveData objData)
    {
        base.Load(data, objData);

        currentFade = data.floatVal;
    }
}
